using Xbim.Common.Geometry;

namespace IFC.Tools.Geometry
{
    public struct IfcAxisSettings
    {
        public XbimVector3D Origin;
        public XbimVector3D XAxis;
        public XbimVector3D YAxis;
        public XbimVector3D ZAxis;
        
        public IfcAxisSettings(XbimVector3D xAxis)
        {
            XAxis = xAxis.Normalized();
            YAxis = new XbimVector3D(xAxis.Y, xAxis.Z, xAxis.X).Normalized();
            ZAxis = XbimVector3D.CrossProduct(XAxis, YAxis).Normalized();
            Origin = XbimVector3D.Zero;
        }

        public IfcAxisSettings(XbimVector3D xAxis, XbimVector3D yAxis)
        {
            XAxis = xAxis.Normalized();
            YAxis = yAxis.Normalized();
            ZAxis = XbimVector3D.CrossProduct(XAxis, YAxis).Normalized();
            Origin = XbimVector3D.Zero;
        }

        public IfcAxisSettings(XbimVector3D origin, XbimVector3D xAxis, XbimVector3D yAxis)
        {
            XAxis = xAxis.Normalized();
            YAxis = yAxis.Normalized();
            ZAxis = XbimVector3D.CrossProduct(XAxis, YAxis).Normalized();
            Origin = origin;
        }
    }
}