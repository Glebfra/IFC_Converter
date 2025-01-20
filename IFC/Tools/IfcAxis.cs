using IFC_Converter.IFC.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometryResource;

namespace IFC_Converter.IFC.Tools;

public static class IfcAxis
{
    public static IfcCartesianPoint CreatePoint(IModel model, XbimVector3D coordinates)
    {
        return model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(coordinates.X, coordinates.Y, coordinates.Z));
    }
    
    public static IfcDirection CreateDirection(IModel model, XbimVector3D direction)
    {
        return model.Instances.New<IfcDirection>(d => d.SetXYZ(direction.X, direction.Y, direction.Z));
    }

    public static XbimVector3D GetDirectionToPipe(IfcPipeEntity pipeEntity, XbimVector3D Coordinates)
    {
        XbimVector3D pipeStartCoordinates = pipeEntity.ObjectMatrix3D.Translation;
        XbimVector3D pipeDirection = pipeEntity.ObjectMatrix3D.Forward;
        XbimVector3D pipeEndCoordinates = pipeStartCoordinates + pipeDirection;
        return (pipeStartCoordinates - Coordinates).Length < (pipeEndCoordinates - Coordinates).Length
            ? pipeDirection
            : pipeDirection * -1;
    }

    public static IfcLocalPlacement CreateLocalPlacement(IModel model, XbimVector3D coordinates, XbimVector3D direction)
    {
        return model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = CreateAxis2Placement3D(model, coordinates, direction);
        });
    }

    public static IfcLocalPlacement CreateLocalPlacement(IModel model, XbimVector3D coordinates)
    {
        return model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = CreateAxis2Placement3D(model, coordinates);
        });
    }

    public static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, XbimVector3D coordinates)
    {
        return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
        {
            placement2D.Location = CreatePoint(model, coordinates);
        });
    }

    public static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, XbimVector3D coordinates, XbimVector3D direction)
    {
        return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
        {
            placement2D.Location = CreatePoint(model, coordinates);
            placement2D.RefDirection = CreateDirection(model, direction);
        });
    }
    
    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, XbimVector3D coordinates, XbimVector3D direction, XbimVector3D refDirection)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = CreatePoint(model, coordinates);
            placement3D.Axis = CreateDirection(model, direction);
            placement3D.RefDirection = CreateDirection(model, refDirection);
        });
    }

    public static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, XbimVector3D coordinates, XbimVector3D direction)
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
}