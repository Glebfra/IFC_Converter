using System;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Extensions
{
    public static class VectorExtensions
    {
        public static XbimVector3D Right => X;
        public static XbimVector3D Up => Y;
        public static XbimVector3D Forward => Z;

        public static XbimVector3D X => new XbimVector3D(1, 0, 0);
        public static XbimVector3D Y => new XbimVector3D(0, 1, 0);
        public static XbimVector3D Z => new XbimVector3D(0, 0, 1);
    
        public static double SignedAngle(this XbimVector3D first, XbimVector3D second)
        {
            return Math.Acos(XbimVector3D.DotProduct(first, second) / (first.Length * second.Length));
        }
        
        public static IfcDirection ToIfcDirection(this XbimVector3D vector, IModel model)
        {
            return IfcAxis.CreateDirection(model, vector);
        }
    }
}