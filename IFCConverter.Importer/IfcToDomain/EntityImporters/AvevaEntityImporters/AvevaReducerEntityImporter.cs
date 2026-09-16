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
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

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
            
            Matrix<double> objToWorldMat = avevaPset.Ori;
            Matrix<double> worldToObjMat = objToWorldMat.Inverse();
            Vector<double> position = avevaPset.Pos * lengthPower;
            
            Vector<double>[] globalVertices = faceSet.Coordinates.GetCoordinates().Select(vertex => vertex * lengthPower).ToArray();
            Vector<double>[] localVertices = globalVertices.Select(objToWorldMat.LeftMultiply).ToArray();
            
            Vector<double> localMinPoint = GetLocalMinPoint(localVertices);
            Vector<double> localMaxPoint = GetLocalMaxPoint(localVertices);
            
            Vector<double>[] firstCircleLocalPoints = localVertices
                .Where(vertex => Math.Abs(vertex[0] - localMinPoint[0]) < DoubleTolerance)
                .ToArray();
            Vector<double>[] secondCircleLocalPoints = localVertices
                .Where(vertex => Math.Abs(vertex[0] - localMaxPoint[0]) < DoubleTolerance)
                .ToArray();
            
            Vector<double> firstCircleLocalCenterPoint = firstCircleLocalPoints.Average();
            Vector<double> secondCircleLocalCenterPoint = secondCircleLocalPoints.Average();

            Vector<double> centerDisplacement = secondCircleLocalCenterPoint - firstCircleLocalCenterPoint;
            Vector<double> axisDisplacement = centerDisplacement.DotProduct(objToWorldMat.GetZ()) * objToWorldMat.GetZ();
            
            Vector<double>[] boundPoints = new[]
            {
                worldToObjMat.LeftMultiply(firstCircleLocalCenterPoint), worldToObjMat.LeftMultiply(secondCircleLocalCenterPoint)
            };
            double[] diameters = new[]
            {
                (firstCircleLocalPoints[0] - firstCircleLocalCenterPoint).L2Norm() * 2,
                (secondCircleLocalPoints[0] - secondCircleLocalCenterPoint).L2Norm() * 2
            };
            
            double length = (boundPoints[1] - axisDisplacement - boundPoints[0]).L2Norm();
            bool isEccentric = !axisDisplacement.AlmostEqual(VectorExtensions.Zero, DoubleTolerance);
            
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
        
        private static Vector<double> GetLocalMinPoint(IReadOnlyList<Vector<double>> localVertices)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double minZ = double.MaxValue;
            
            foreach (Vector<double> localVertex in localVertices)
            {
                if (localVertex[0] < minX)
                    minX = localVertex[0];
                if (localVertex[1] < minY)
                    minY = localVertex[1];
                if (localVertex[2] < minZ)
                    minZ = localVertex[2];
            }
            
            return new DenseVector(new double[] { minX, minY, minZ });
        }

        private static Vector<double> GetLocalMaxPoint(IReadOnlyList<Vector<double>> localVertices)
        {
            double maxX = double.MinValue;
            double maxY = double.MinValue;
            double maxZ = double.MinValue;
            
            foreach (Vector<double> localVertex in localVertices)
            {
                if (localVertex[0] > maxX)
                    maxX = localVertex[0];
                if (localVertex[1] > maxY)
                    maxY = localVertex[1];
                if (localVertex[2] > maxZ)
                    maxZ = localVertex[2];
            }

            return new DenseVector(new double[] { maxX, maxY, maxZ });
        }
    }
}