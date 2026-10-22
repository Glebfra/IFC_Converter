using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Geometry;
using IFCConverter.Geometry.Algorithms;
using IFCConverter.Geometry.Boundary;
using IFCConverter.Geometry.Mesh;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ReducerEntityImporters
{
    internal sealed class TriangulatedFaceSetReducerEntityImporter : IReducerEntityImporter
    {
        private IIfcLocalPlacement _localPlacement;
        private IIfcTriangulatedFaceSet _triangulatedFaceSet;
        
        private readonly struct DiametersData
        {
            public readonly FixedVector<Dim3> MinDiameterCenter;
            public readonly FixedVector<Dim3> MaxDiameterCenter;
            
            public readonly double MinDiameter;
            public readonly double MaxDiameter;

            public DiametersData(FixedVector<Dim3> minDiameterCenter, FixedVector<Dim3> maxDiameterCenter, double minDiameter, double maxDiameter)
            {
                MinDiameterCenter = minDiameterCenter;
                MaxDiameterCenter = maxDiameterCenter;
                MinDiameter = minDiameter;
                MaxDiameter = maxDiameter;
            }
        }
        
        public bool CanImport(IIfcProduct product)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                return false;
            _localPlacement = localPlacement;
            
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                return false;
            if (!(representationItems[0] is IIfcTriangulatedFaceSet triangulatedFaceSet))
                return false;
            _triangulatedFaceSet = triangulatedFaceSet;

            return true;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            double lengthPower = product.Model.GetLengthPower();

            FixedMatrix<Dim4> globalMatrix = _localPlacement.GetGlobalMatrix();
            FixedVector<Dim3> position = globalMatrix.GetTranslation() * lengthPower;
            FixedMatrix<Dim3> rotation = globalMatrix.GetRotation();
            
            IMesh mesh = _triangulatedFaceSet.GetMesh();
            PlanarRegion[] endRegions = PlanarRegionFinder
                .Find(mesh, distanceTolerance: 1e-3)
                .Where(region => region.TriangleCount >= 3 && region.BoundaryEdges.Count >= 3)
                .OrderByDescending(region => region.Score)
                .Take(2)
                .ToArray();
            FixedVector<Dim3>[] normals = endRegions.Select(region => region.TriangleIndices.Select(index => mesh.FaceNormals[index]).Mean()).ToArray();
            if (endRegions.Length != 2)
                throw new InvalidOperationException($"Expected 2 reducer end regions, found {endRegions.Length}.");

            FixedVector<Dim3>[][] boundariesVertices = endRegions.Select(region =>
            {
                List<BoundaryLoop> loops = BoundaryLoopBuilder.Build(region.BoundaryEdges);
                if (loops.Count != 1)
                    throw new InvalidOperationException($"Expected one boundary loop, found {loops.Count}.");

                return loops[0].Vertices
                    .Select(index => mesh.Vertices[index])
                    .Select(vertex => vertex * rotation * lengthPower + position)
                    .ToArray();
            }).ToArray();
            
            FixedVector<Dim3>[] boundaryCenters = boundariesVertices
                .Select(boundaryVertices => boundaryVertices.Mean())
                .ToArray();
            
            if (boundaryCenters.Length != 2)
                throw new Exception("The given boundary centers are not equal to 2 centroids.");

            double[] diameters = boundariesVertices
                .Select((vertices, index) => vertices
                    .Select(vertex => (vertex - boundaryCenters[index]).L2Norm() * 2)
                    .Average())
                .ToArray();

            DiametersData diametersData = FilterCenters(boundaryCenters, diameters);
            FixedVector<Dim3> reducerDirection = diametersData.MaxDiameterCenter - diametersData.MinDiameterCenter;
            FixedVector<Dim3> reducerAxisProjection = reducerDirection.Dot(normals[0]) * normals[0];
            FixedVector<Dim3> reducerAxisDirection = reducerAxisProjection.SafeNormalize();
            
            Reducer reducer = new Reducer(Id<EntityId>.New())
            {
                Position = diametersData.MinDiameterCenter,
                Length = reducerAxisProjection.L2Norm()
            };
            
            bool isEccentric = !reducerDirection.IsParallel(reducerAxisDirection);
            reducer.TypedMetadata.Axis = reducerAxisDirection;
            reducer.TypedMetadata.IsEccentric = isEccentric;
            
            reducer.PortA.SetGeometry(diametersData.MinDiameterCenter, reducerAxisDirection.Negate());
            reducer.PortB.SetGeometry(diametersData.MaxDiameterCenter, reducerAxisDirection);

            reducer.PortA.Metadata.Diameter = diametersData.MinDiameter;
            reducer.PortB.Metadata.Diameter = diametersData.MaxDiameter;
            
            model.Add(reducer);
            context.Register(reducer, product);
        }

        private static DiametersData FilterCenters(FixedVector<Dim3>[] boundaryCenters, double[] diameters)
        {
            double minDiameter = double.MaxValue, maxDiameter = double.MinValue;
            int minDiameterIndex = 0, maxDiameterIndex = 0;
            
            for (int i = 0; i < diameters.Length; i++)
            {
                if (diameters[i] < minDiameter)
                {
                    minDiameterIndex = i;
                    minDiameter = diameters[i];
                }

                if (diameters[i] > maxDiameter)
                {
                    maxDiameterIndex = i;
                    maxDiameter = diameters[i];
                }
            }

            return new DiametersData(
                boundaryCenters[minDiameterIndex], boundaryCenters[maxDiameterIndex],
                minDiameter, maxDiameter
            );
        }
    }
}