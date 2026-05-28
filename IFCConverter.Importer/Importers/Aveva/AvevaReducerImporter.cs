using System;
using System.Collections.Generic;
using System.Linq;
using Ifc.Extensions;
using IFCConverter.Importer.Entities.Proxies;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.Interfaces;
using IFCConverter.Importer.PropertySets.Aveva;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Utils;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Importer.Importers.Aveva
{
    internal class AvevaReducerImporter : AbstractEntityImporter<IfcBuildingElementProxy, ReducerProxy>
    {
        private const double _tolerance = 1e-6;
        private readonly VectorComparer _comparer = new VectorComparer(_tolerance);
        
        public override ReducerProxy ReadTyped(IfcBuildingElementProxy source)
        {
            IIfcRepresentationItem[] representationItems = GetRepresentationItems(source).ToArray();
            if (representationItems.Length != 1)
                throw new Exception("Expected exactly one representation item for the given source.");
            
            if (representationItems[0] is not IIfcTriangulatedFaceSet faceSet)
                throw new Exception("The representation item is not a triangulated face set.");

            IEnumerable<IPropertySet> propertySets = source.GetPropertySets();
            AvevaPset? avevaPset = propertySets.OfType<AvevaPset>().FirstOrDefault();
            if (avevaPset == null)
                throw new Exception("The required Aveva property set is missing.");

            Matrix<double> objToWorldMat = avevaPset.Ori;
            Matrix<double> worldToObjMat = objToWorldMat.Inverse();
            Vector<double> position = avevaPset.Pos;
            
            Vector<double>[] globalVertices = faceSet.Coordinates.GetCoordinates().ToArray();
            Vector<double>[] localVertices = globalVertices
                .Select(vertex => objToWorldMat.LeftMultiply(vertex))
                .ToArray();

            Vector<double> localMinPoint = GetLocalMinPoint(localVertices);
            Vector<double> localMaxPoint = GetLocalMaxPoint(localVertices);

            Vector<double>[] firstCircleLocalPoints = localVertices
                .Where(vertex => Math.Abs(vertex[0] - localMinPoint[0]) < _tolerance)
                .ToArray();
            Vector<double>[] secondCircleLocalPoints = localVertices
                .Where(vertex => Math.Abs(vertex[0] - localMaxPoint[0]) < _tolerance)
                .ToArray();

            Vector<double> firstCircleLocalCenterPoint = firstCircleLocalPoints.Average();
            Vector<double> secondCircleLocalCenterPoint = secondCircleLocalPoints.Average();

            Vector<double> centerDisplacement = secondCircleLocalCenterPoint - firstCircleLocalCenterPoint;
            Vector<double> axisDisplacement = centerDisplacement
                .DotProduct(objToWorldMat.GetZ()) * objToWorldMat.GetZ();

            IReadOnlyList<Vector<double>> boundPoints = new Vector<double>[]
            {
                worldToObjMat.LeftMultiply(firstCircleLocalCenterPoint),
                worldToObjMat.LeftMultiply(secondCircleLocalCenterPoint)
            };

            bool isEccentric = !_comparer.Equals(axisDisplacement, VectorExtensions.Zero);
            double length = (boundPoints[1] - axisDisplacement - boundPoints[0]).L2Norm();
            double lengthPower = GetLengthPower(source);

            return new ReducerProxy(
                position: boundPoints[1] * lengthPower,
                boundPoints: boundPoints.Select(point => point * lengthPower).ToArray(),
                isEccentric: isEccentric,
                length: length * lengthPower
            )
            {
                Name = source.Name
            };
        }
        
        private static Vector<double> GetLocalMinPoint(IReadOnlyList<Vector<double>> localVertices)
        {
            return new DenseVector(new double[]
            {
                localVertices.Min(vertex => vertex[0]),
                localVertices.Min(vertex => vertex[1]),
                localVertices.Min(vertex => vertex[2]),
            });
        }
        
        private static Vector<double> GetLocalMaxPoint(IReadOnlyList<Vector<double>> localVertices)
        {
            return new DenseVector(new double[]
            {
                localVertices.Max(vertex => vertex[0]),
                localVertices.Max(vertex => vertex[1]),
                localVertices.Max(vertex => vertex[2]),
            });
        }
    }
}