using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.ProfileResource;

namespace IFC.Tools
{
    public static partial class IfcGeometry
    {
        public static IfcPlane CreatePlane(IModel model, XbimVector3D coordinates, XbimVector3D direction)
        {
            return model.Instances.New<IfcPlane>(plane =>
            {
                plane.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates, direction);
            });
        }

        public static IfcSweptDiskSolid CreateCircularBend(IModel model, double circleRadius, double bendRadius, double angle, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            axis = axis.Normalized();
            refDirection = refDirection.Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(axis, refDirection).Normalized();
            
            IfcCircle circle = CreateCircle(model, bendRadius, coordinates, up, refDirection);
            IfcTrimmedCurve curve = CreateTrimmedCurve(model, circle, -angle, 0);
            return CreateSweptDiskSolid(model, curve, circleRadius);
        }

        public static IfcExtrudedAreaSolid CreateRectangle(IModel model, double xDim, double yDim, double zDim, XbimVector3D coordinates)
        {
            IfcRectangleProfileDef rectangleProfileDef = CreateRectangleProfileDef(model, xDim, yDim);
            return CreateExtrudedAreaSolid(model, rectangleProfileDef, zDim, coordinates);
        }

        public static IfcExtrudedAreaSolid CreateCylinder(IModel model, double radius, double zDim, XbimVector3D coordinates)
        {
            IfcCircleProfileDef circleProfileDef = CreateCircleProfileDef(model, radius);
            return CreateExtrudedAreaSolid(model, circleProfileDef, zDim, coordinates);
        }
        
        public static IfcExtrudedAreaSolid CreateRectangle(IModel model, double xDim, double yDim, double zDim, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            IfcRectangleProfileDef rectangleProfileDef = CreateRectangleProfileDef(model, xDim, yDim);
            return CreateExtrudedAreaSolid(model, rectangleProfileDef, zDim, coordinates, axis, refDirection);
        }

        public static IfcExtrudedAreaSolid CreateCylinder(IModel model, double radius, double zDim, XbimVector3D coordinates, XbimVector3D axis, XbimVector3D refDirection)
        {
            IfcCircleProfileDef circleProfileDef = CreateCircleProfileDef(model, radius);
            return CreateExtrudedAreaSolid(model, circleProfileDef, zDim, coordinates, axis, refDirection);
        }
    }
}