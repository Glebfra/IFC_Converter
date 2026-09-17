using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Geometry;
using IFCConverter.Geometry.MeshResolvers;
using IFCConverter.IFC.Extensions;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Mathematics;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.AvevaEntityImporters
{
    internal sealed class AvevaReducerEntityImporter : IAvevaEntityImporter
    {
        private const double DoubleTolerance = 1e-6;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);

        public bool CanImport(IIfcProduct product, AvevaEntityType type)
        {
            return type == AvevaEntityType.REDUCER;
        }

        public void Import(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return;
            IIfcRepresentationItem[] representationItems = product.GetRepresentationItems().ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");

            if (!(representationItems[0] is IIfcTriangulatedFaceSet faceSet))
                throw new Exception("The representation item is not a triangulated face set.");

            IEnumerable<IPropertySet> propertySets = ((IfcProduct)product).GetPropertySets();
            AvevaPset avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            IMesh mesh = faceSet.GetMesh();
            ClosedClippedConeMeshResolver resolver = new ClosedClippedConeMeshResolver();
            resolver.Resolve(mesh);

            double lengthPower = product.Model.GetLengthPower();

            FixedMatrix<Dim3> objToWorldMat = avevaPset.Ori;
            FixedMatrix<Dim3> worldToObjMat = objToWorldMat.Inverse();
            FixedVector<Dim3> position = avevaPset.Pos * lengthPower;

            FixedVector<Dim3>[] globalVertices = faceSet.Coordinates.GetFixedCoordinates().Select(vertex => vertex * lengthPower).ToArray();
            FixedVector<Dim3>[] localVertices = globalVertices.Select(objToWorldMat.LeftMultiply).ToArray();

            FixedVector<Dim3> localMinPoint = GetLocalMinPoint(localVertices);
            FixedVector<Dim3> localMaxPoint = GetLocalMaxPoint(localVertices);

            FixedVector<Dim3>[] firstCircleLocalPoints = localVertices
                .Where(vertex => Math.Abs(vertex[0] - localMinPoint[0]) < DoubleTolerance)
                .ToArray();
            FixedVector<Dim3>[] secondCircleLocalPoints = localVertices
                .Where(vertex => Math.Abs(vertex[0] - localMaxPoint[0]) < DoubleTolerance)
                .ToArray();

            FixedVector<Dim3> firstCircleLocalCenterPoint = firstCircleLocalPoints.Average();
            FixedVector<Dim3> secondCircleLocalCenterPoint = secondCircleLocalPoints.Average();

            FixedVector<Dim3> centerDisplacement = secondCircleLocalCenterPoint - firstCircleLocalCenterPoint;
            FixedVector<Dim3> axisDisplacement = centerDisplacement.Dot(objToWorldMat.GetZ()) * objToWorldMat.GetZ();

            FixedVector<Dim3>[] boundPoints =
            {
                worldToObjMat.LeftMultiply(firstCircleLocalCenterPoint), worldToObjMat.LeftMultiply(secondCircleLocalCenterPoint)
            };
            double[] diameters =
            {
                (firstCircleLocalPoints[0] - firstCircleLocalCenterPoint).L2Norm() * 2,
                (secondCircleLocalPoints[0] - secondCircleLocalCenterPoint).L2Norm() * 2
            };

            double length = (boundPoints[1] - axisDisplacement - boundPoints[0]).L2Norm();
            bool isEccentric = !axisDisplacement.AlmostEqual(FixedVector<Dim3>.Zeros());

            Reducer reducer = new Reducer(EntityId.New())
            {
                Position = position,
                Length = length
            };
            reducer.Metadata.Meta.Add("BoundPoints", boundPoints);
            reducer.Metadata.Meta.Add("Diameters", diameters);
            reducer.Metadata.Meta.Add("IsEccentric", isEccentric);

            model.Add(reducer);
            context.Register(reducer, product);
        }

        private static FixedVector<Dim3> GetLocalMinPoint(IReadOnlyList<FixedVector<Dim3>> localVertices)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;

            foreach (FixedVector<Dim3> localVertex in localVertices)
            {
                if (localVertex[0] < minX)
                    minX = localVertex[0];
                if (localVertex[1] < minY)
                    minY = localVertex[1];
                if (localVertex[2] < minZ)
                    minZ = localVertex[2];
            }

            return FixedVector<Dim3>.Builder.Dense(minX, minY, minZ);
        }

        private static FixedVector<Dim3> GetLocalMaxPoint(IReadOnlyList<FixedVector<Dim3>> localVertices)
        {
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            double maxZ = double.MinValue;

            foreach (FixedVector<Dim3> localVertex in localVertices)
            {
                if (localVertex[0] > maxX)
                    maxX = localVertex[0];
                if (localVertex[1] > maxY)
                    maxY = localVertex[1];
                if (localVertex[2] > maxZ)
                    maxZ = localVertex[2];
            }

            return FixedVector<Dim3>.Builder.Dense(maxX, maxY, maxZ);
        }
    }
}