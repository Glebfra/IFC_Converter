using System.Linq;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Tools
{
    public static partial class IfcGeometry
    {
        public static IfcTriangulatedFaceSet CreateTriangulatedFaceSet(IModel model, IfcTriangulatedFaceSetProperties properties)
        {
            IfcCartesianPointList3D cartesianPointList3D = model.Instances.New<IfcCartesianPointList3D>(pl =>
            {
                for (int i = 0; i < properties.Vertices.Length; i++)
                {
                    XbimVector3D vertex = properties.Vertices[i];
                    pl.CoordList.GetAt(i).AddRange(new IfcLengthMeasure[] { vertex.X, vertex.Y, vertex.Z });
                }
            });

            IfcTriangulatedFaceSet triangulatedFaceSet = model.Instances.New<IfcTriangulatedFaceSet>(set =>
            {
                set.Coordinates = cartesianPointList3D;

                for (int i = 0; i < properties.Indices.Length; i++)
                {
                    int[] indices = properties.Indices[i];
                    set.CoordIndex.GetAt(i).AddRange(indices.Select(index => new IfcPositiveInteger(index)));
                }
            });

            if (properties.Normals != null)
            {
                for (int i = 0; i < properties.Normals.Length; i++)
                {
                    XbimVector3D normal = properties.Normals[i];
                    triangulatedFaceSet.Normals.GetAt(i).AddRange(new IfcParameterValue[] { normal.X, normal.Y, normal.Z });
                }
            }

            return triangulatedFaceSet;
        }
    }
}