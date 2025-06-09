using System.Collections.Generic;
using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Extensions
{
    public static class IfcTriangulatedFaceSetExtensions
    {
        public static void AddVertices(this IfcTriangulatedFaceSet triangulatedFaceSet, IEnumerable<IEnumerable<int>> vertices, ref int index)
        {
            foreach (IEnumerable<int> vertex in vertices)
            {
                IItemSet<IfcPositiveInteger> indexesList = triangulatedFaceSet.CoordIndex.GetAt(index++);
                indexesList.AddRange(vertex.Select(i => new IfcPositiveInteger(i)));
            }
        }
        
        public static void AddNormals(this IfcTriangulatedFaceSet triangulatedFaceSet, IEnumerable<XbimVector3D> normals, ref int index)
        {
            foreach (XbimVector3D normal in normals)
            {
                double[] doubleNormal = normal.ToDoubleArray();
                IItemSet<IfcParameterValue> indexesList = triangulatedFaceSet.Normals.GetAt(index++);
                indexesList.AddRange(doubleNormal.Select(i => new IfcParameterValue(i)));
            }
        }
    }
}