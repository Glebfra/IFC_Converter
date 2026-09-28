using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Geometry.Boundary;
using IFCConverter.Geometry.Mesh;
using IFCConverter.IFC.Extensions;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.UnknownEntityImporters
{
    internal sealed class UnknownElbowEntityImporter : IUnknownEntityImporter
    {
        public bool CanImport(IIfcProduct product)
        {
            return product is IIfcPipeFitting pipeFitting && 
                   pipeFitting.PredefinedType == IfcPipeFittingTypeEnum.BEND;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!(product.ObjectPlacement is IIfcLocalPlacement localPlacement))
                throw new Exception("The given product is not a local placement.");
            
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new ArgumentException("Expected exactly one representation item", nameof(representationItems));
            
            if (!(representationItems[0] is IIfcTriangulatedFaceSet faceSet))
                throw new Exception("The representation item is not a triangulated face set.");
            
            double lengthPower = product.Model.GetLengthPower();
            
            FixedMatrix<Dim4> globalMatrix = FixedMatrix<Dim4>.Identity();
            while (localPlacement != null)
            {
                FixedMatrix<Dim4> localMatrix = localPlacement.RelativePlacement.ToFixedMatrix();
                globalMatrix = localMatrix * globalMatrix;
                localPlacement = localPlacement.PlacementRelTo as IIfcLocalPlacement;
            }
            FixedVector<Dim3> position = globalMatrix.GetTranslation() * lengthPower;
            FixedMatrix<Dim3> rotation = globalMatrix.GetRotation();

            IMesh mesh = faceSet.GetMesh();
            FixedVector<Dim3>[][] boundariesVertices = GetBoundaryVertices(mesh)
                .Select(vertices => vertices
                    .Select(vertex => vertex * rotation + position)
                    .ToArray())
                .ToArray();
            FixedVector<Dim3>[] boundaryCenters = boundariesVertices
                .Select(boundaryVertices => boundaryVertices.Mean())
                .ToArray();
            if (boundaryCenters.Length != 2)
                throw new Exception("The given boundary centers are not equal to 2 centroids.");

            Elbow elbow = new Elbow(EntityId.New())
            {
                Position = position,
                Radius = CalculateRadius(boundaryCenters, position)
            };

            elbow.Metadata.Meta.Add("BoundariesVertices", boundariesVertices);
            elbow.Metadata.Meta.Add("BoundaryCenters", boundaryCenters);
            
            model.Add(elbow);
            context.Register(elbow, product);
        }

        [Pure]
        private static FixedVector<Dim3>[][] GetBoundaryVertices(IMesh mesh)
        {
            Edge[] boundaryEdges = MeshTopology.GetBoundaryEdges(mesh).ToArray();
            List<BoundaryLoop> boundaryLoops = BoundaryLoopBuilder.Build(boundaryEdges);
            return boundaryLoops
                .Select(boundaryLoop => boundaryLoop.Vertices.Select(index => mesh.Vertices[index]).ToArray())
                .ToArray();
        }

        [Pure]
        private static double CalculateRadius(FixedVector<Dim3>[] boundaryCenters, FixedVector<Dim3> position)
        {
            FixedVector<Dim3> A = boundaryCenters[0];
            FixedVector<Dim3> B = position;
            FixedVector<Dim3> C = boundaryCenters[1];
            
            FixedVector<Dim3> BA = A - B;
            FixedVector<Dim3> BC = C - B;

            double ABNormSquared = BA.Dot(BA);
            double BCNormSquared = BC.Dot(BC);
            
            double ABNorm = Math.Sqrt(ABNormSquared);
            double BCNorm = Math.Sqrt(BCNormSquared);
            
            double cosAngle = BA.Dot(BC) / (ABNorm * BCNorm);

            double nominator = ABNormSquared + BCNormSquared - 2 * ABNorm * BCNorm * cosAngle;
            double denominator = 2 * (1 + cosAngle);

            return Math.Sqrt(nominator / denominator);
        }
    }
}