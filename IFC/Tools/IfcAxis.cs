using IFC_Converter.IFC.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC_Converter.IFC.Tools;

public static class IfcAxis
{

    #region Point

    public static IfcCartesianPoint CreatePoint(IModel model, XbimVector3D coordinates)
    {
        return model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(coordinates.X, coordinates.Y, coordinates.Z));
    }

    #endregion

    #region Direction

    public static IfcDirection CreateDirection(IModel model, XbimVector3D direction)
    {
        return model.Instances.New<IfcDirection>(d => d.SetXYZ(direction.X, direction.Y, direction.Z));
    }
    
    public static XbimVector3D GetDirectionToPipe(IfcPipeEntity pipeEntity, XbimVector3D Coordinates)
    {
        XbimVector3D pipeStartCoordinates = pipeEntity.ObjectMatrix3D.Translation;
        XbimVector3D pipeDirection = pipeEntity.ObjectMatrix3D.Forward;
        double pipeLength = pipeEntity.Depth;
        XbimVector3D pipeEndCoordinates = pipeStartCoordinates + pipeDirection * pipeLength;
        return (pipeStartCoordinates - Coordinates).Length < (pipeEndCoordinates - Coordinates).Length
            ? pipeDirection
            : pipeDirection * -1;
    }

    #endregion

    #region LocalPlacement

    public static IfcLocalPlacement CreateLocalPlacement(IModel model, IfcAxis2Placement3D axis2Placement3D)
    {
        return model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = axis2Placement3D;
        });
    }

    #endregion

    #region Axis
    
    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, IfcCartesianPoint point, IfcDirection axis, IfcDirection refDirection)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = point;
            placement3D.Axis = axis;
            placement3D.RefDirection = refDirection;
        });
    }
    
    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, IfcCartesianPoint point)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = point;
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

    public static IfcAxis1Placement CreateAxis1Placement(IModel model, XbimVector3D coordinates, XbimVector3D direction)
    {
        return model.Instances.New<IfcAxis1Placement>(placement =>
        {
            placement.Location = CreatePoint(model, coordinates);
            placement.Axis = CreateDirection(model, direction);
        });
    }

    public static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, XbimVector3D coordinates)
    {
        return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
        {
            placement2D.Location = CreatePoint(model, coordinates);
        });
    }

    public static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, XbimVector3D coordinates,
        XbimVector3D direction)
    {
        return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
        {
            placement2D.Location = CreatePoint(model, coordinates);
            placement2D.RefDirection = CreateDirection(model, direction);
        });
    }

    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, XbimVector3D coordinates,
        XbimVector3D direction, XbimVector3D refDirection)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = CreatePoint(model, coordinates);
            placement3D.Axis = CreateDirection(model, direction);
            placement3D.RefDirection = CreateDirection(model, refDirection);
        });
    }

    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, XbimVector3D coordinates,
        XbimVector3D direction)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = CreatePoint(model, coordinates);
            placement3D.Axis = CreateDirection(model, direction);
            placement3D.RefDirection = CreateDirection(model, new XbimVector3D(direction.Y, direction.Z, direction.X));
        });
    }

    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, XbimVector3D coordinates)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = CreatePoint(model, coordinates);
        });
    }

    #endregion
}