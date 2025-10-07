using Xbim.Common.Geometry;

namespace IFC.Tools
{
    public struct BendProperties
    {
        public XbimVector3D[] BoundPoints;
        public XbimVector3D Center;
        public double Radius;
        public double Angle;
        public double PipeDiameter;
    }

    public struct ReducerProperties
    {
        public XbimVector3D[] BoundPoints;
        public XbimVector3D Center;
        public double[] Radiuses;
        public double Length;
    }
}