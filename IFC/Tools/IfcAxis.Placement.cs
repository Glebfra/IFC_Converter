using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC.Tools
{
    public static partial class IfcAxis
    {
        public static IfcObjectPlacement CreatePointObjectPlacement(IModel model, ActionProperty<XbimMatrix3D> ObjectMatrix3D)
        {
            ActionProperty<XbimVector3D> coordinates = ObjectMatrix3D.Value.Translation;
            ObjectMatrix3D.OnValueChange += () => coordinates.Value = ObjectMatrix3D.Value.Translation;
            
            IfcCartesianPoint point = CreatePoint(model, coordinates);
            IfcAxis2Placement3D axis2Placement3D = CreateAxis2Placement3D(model, point);
            IfcLocalPlacement localPlacement = CreateLocalPlacement(model, axis2Placement3D);
            
            return new IfcObjectPlacement()
            {
                Point = point,
                Axis2Placement3D = axis2Placement3D,
                LocalPlacement = localPlacement
            };
        }

        public static IfcObjectPlacement CreatePointAndDirectionsObjectPlacement(IModel model, ActionProperty<XbimMatrix3D> ObjectMatrix3D)
        {
            ActionProperty<XbimVector3D> pointVector = ObjectMatrix3D.Value.Translation;
            ActionProperty<XbimVector3D> forwardVector = ObjectMatrix3D.Value.Forward;
            ActionProperty<XbimVector3D> rightVector = ObjectMatrix3D.Value.Right;

            ObjectMatrix3D.OnValueChange += () => pointVector.Value = ObjectMatrix3D.Value.Translation;
            ObjectMatrix3D.OnValueChange += () => forwardVector.Value = ObjectMatrix3D.Value.Forward;
            ObjectMatrix3D.OnValueChange += () => rightVector.Value = ObjectMatrix3D.Value.Right;

            IfcCartesianPoint point = CreatePoint(model, pointVector);
            IfcDirection forward = CreateDirection(model, forwardVector);
            IfcDirection right = CreateDirection(model, rightVector);

            IfcAxis2Placement3D axis2Placement3D = CreateAxis2Placement3D(model, point, forward, right);
            IfcLocalPlacement localPlacement = CreateLocalPlacement(model, axis2Placement3D);
            
            return new IfcObjectPlacement()
            {
                Point = point,
                Forward = forward,
                Right = right,
                Axis2Placement3D = axis2Placement3D,
                LocalPlacement = localPlacement
            };
        }

        public static IfcLocalPlacement CreateLocalPlacement(IModel model, IfcAxis2Placement3D axis2Placement3D)
        {
            return model.Instances.New<IfcLocalPlacement>(lp =>
            {
                lp.RelativePlacement = axis2Placement3D;
            });
        }
        
        public static IfcAxis1Placement CreateAxis1Placement(IModel model, IfcCartesianPoint location, IfcDirection axis)
        {
            return model.Instances.New<IfcAxis1Placement>(placement =>
            {
                placement.Location = location;
                placement.Axis = axis;
            });
        }

        public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, IfcCartesianPoint point, IfcDirection axis, IfcDirection refDirection)
        {
            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = point;
                placement3D.Axis = axis;
                placement3D.RefDirection = refDirection;
            });
        }
        
        public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, ActionProperty<XbimVector3D> coordinates)
        {
            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = CreatePoint(model, coordinates);
            });
        }
    
        public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, IfcCartesianPoint point)
        {
            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = point;
            });
        }

        public static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, ActionProperty<XbimVector3D> coordinates)
        {
            return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
            {
                placement2D.Location = CreatePoint(model, coordinates);
            });
        }

        public static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, ActionProperty<XbimVector3D> coordinates, ActionProperty<XbimVector3D> direction)
        {
            return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
            {
                placement2D.Location = CreatePoint(model, coordinates);
                placement2D.RefDirection = CreateDirection(model, direction);
            });
        }

        public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, ActionProperty<XbimVector3D> coordinates, ActionProperty<XbimVector3D> direction, ActionProperty<XbimVector3D> refDirection)
        {
            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = CreatePoint(model, coordinates);
                placement3D.Axis = CreateDirection(model, direction);
                placement3D.RefDirection = CreateDirection(model, refDirection);
            });
        }

        public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, ActionProperty<XbimVector3D> coordinates, ActionProperty<XbimVector3D> direction)
        {
            ActionProperty<XbimVector3D> refDirection = new XbimVector3D(direction.Value.Y, direction.Value.Z, direction.Value.X);
            direction.OnValueChange += () => refDirection.Value = new XbimVector3D(direction.Value.Y, direction.Value.Z, direction.Value.X);

            return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
            {
                placement3D.Location = CreatePoint(model, coordinates);
                placement3D.Axis = CreateDirection(model, direction);
                placement3D.RefDirection = CreateDirection(model, refDirection);
            });
        }
    }
}