using Xbim.Common.Geometry;

namespace IFC.Tools
{
    public struct IfcTriangulatedFaceSetProperties
    {
        public XbimVector3D[] Vertices;
        public int[][] Indices;
        public XbimVector3D[]? Normals;
    }
}