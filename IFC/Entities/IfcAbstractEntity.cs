using IFC_Converter.Math;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public abstract class IfcAbstractEntity
{
    public abstract IfcObject CreateAndAdd(IModel model);

    #region Geometric Axis Representation

    protected static IfcCartesianPoint CreatePoint(IModel model, Vector3 coordinates)
    {
        return model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(coordinates.x, coordinates.y, coordinates.z));
    }
    
    protected static IfcDirection CreateDirection(IModel model, Vector3 direction)
    {
        return model.Instances.New<IfcDirection>(d => d.SetXYZ(direction.x, direction.y, direction.z));
    }

    protected static Vector3 GetRightPipeDirection(StartPipeEntity pipeEntity, Vector3 Coordinates)
    {
        Vector3 pipeStartCoordinates = pipeEntity.GetCoordinates();
        Vector3 pipeDirection = pipeEntity.GetDirection();
        Vector3 pipeEndCoordinates = pipeStartCoordinates + pipeDirection;
        return (pipeStartCoordinates - Coordinates).Length < (pipeEndCoordinates - Coordinates).Length
            ? pipeDirection
            : pipeDirection * -1;
    }

    protected static IfcLocalPlacement CreateLocalPlacement(IModel model, Vector3 coordinates, Vector3 direction)
    {
        return model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = CreateAxis2Placement3D(model, coordinates, direction);
        });
    }

    protected static IfcLocalPlacement CreateLocalPlacement(IModel model, Vector3 coordinates)
    {
        return model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = CreateAxis2Placement3D(model, coordinates);
        });
    }

    protected static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, Vector3 coordinates)
    {
        return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
        {
            placement2D.Location = CreatePoint(model, coordinates);
        });
    }

    protected static IfcAxis2Placement2D CreateAxis2Placement2D(IModel model, Vector3 coordinates, Vector3 direction)
    {
        return model.Instances.New<IfcAxis2Placement2D>(placement2D =>
        {
            placement2D.Location = CreatePoint(model, coordinates);
            placement2D.RefDirection = CreateDirection(model, direction);
        });
    }

    protected static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, Vector3 coordinates, Vector3 direction)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = CreatePoint(model, coordinates);
            placement3D.Axis = CreateDirection(model, direction.XYZ);
            placement3D.RefDirection = CreateDirection(model, direction.YZX);
        });
    }
    
    protected static IfcAxis2Placement3D CreateAxis2Placement3D(IModel model, Vector3 coordinates)
    {
        return model.Instances.New<IfcAxis2Placement3D>(placement3D =>
        {
            placement3D.Location = CreatePoint(model, coordinates);
        });
    }

    #endregion

    #region Geometric Figures Representation

    protected static IfcPlane CreatePlane(IModel model, Vector3 coordinates, Vector3 direction)
    {
        return model.Instances.New<IfcPlane>(plane =>
        {
            plane.Position = CreateAxis2Placement3D(model, coordinates, direction);
        });
    }
    
    protected static IfcTrimmedCurve CreateTrimmedCurve(IModel model, IfcCurve basicCurve, double firstParameter, double secondParameter)
    {
        return model.Instances.New<IfcTrimmedCurve>(curve =>
        {
            curve.BasisCurve = basicCurve;
            curve.Trim1.Add(new IfcParameterValue(firstParameter));
            curve.Trim2.Add(new IfcParameterValue(secondParameter));
            curve.SenseAgreement = true;
            curve.MasterRepresentation = IfcTrimmingPreference.PARAMETER;
        });
    }

    protected static IfcTrimmedCurve CreateTrimmedCurve(IModel model, IfcCurve basicCurve, Vector3 firstPoint, Vector3 secondPoint)
    {
        return model.Instances.New<IfcTrimmedCurve>(curve =>
        {
            curve.BasisCurve = basicCurve;
            curve.Trim1.Add(CreatePoint(model, firstPoint));
            curve.Trim2.Add(CreatePoint(model, secondPoint));
            curve.SenseAgreement = true;
            curve.MasterRepresentation = IfcTrimmingPreference.CARTESIAN;
        });
    }

    protected static IfcCircle CreateCircle(IModel model, double radius, Vector3 coordinates, Vector3 direction)
    {
        return model.Instances.New<IfcCircle>(ifcCircle =>
        {
            ifcCircle.Radius = radius;
            ifcCircle.Position = CreateAxis2Placement3D(model, coordinates, direction);
        });
    }
    
    protected static IfcCircleProfileDef CreateCircleProfileDef(IModel model, double radius, Vector3 coordinates, Vector3 direction)
    {
        return model.Instances.New<IfcCircleProfileDef>(def =>
        {
            def.ProfileType = IfcProfileTypeEnum.AREA;
            def.Radius = radius;
            def.Position = CreateAxis2Placement2D(model, coordinates, direction);
        });
    }

    protected static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem representationItem)
    {
        return model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(representationItem);
        });
    }

    protected static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem[] representationItems)
    {
        return model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.AddRange(representationItems);
        });
    }

    protected static IfcProductDefinitionShape CreateProductDefinitionShape(IModel model, IfcShapeRepresentation shapeRepresentation)
    {
        return model.Instances.New<IfcProductDefinitionShape>(shape => shape.Representations.Add(shapeRepresentation));
    }

    #endregion

    #region Properties Representation

    protected static IfcRelDefinesByProperties AddProperties(IModel model, IfcObject ifcObject, StartAbstractEntity entity)
    {
        return model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(ifcObject);
            rel.RelatingPropertyDefinition = CreatePropertySet(model, "Properties", entity);
        });
    }

    protected static IfcPropertySet CreatePropertySet(IModel model, string name, StartAbstractEntity entity)
    {
        return model.Instances.New<IfcPropertySet>(set =>
        {
            set.Name = name;
            foreach (var kvp in entity.GetData())
            {
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(prop =>
                {
                    prop.Name = kvp.Key;
                    prop.NominalValue = new IfcText(kvp.Value);
                }));
            }
        });
    }

    #endregion

    #region Segments Representation

    protected static IfcPipeSegment CreatePipeSegment(IModel model, string name, IfcLocalPlacement placement, IfcProductRepresentation representation)
    {
        return model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = name;
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = placement;
            p.Representation = representation;
        });
    }

    protected static IfcDistributionPort CreatePort(IModel model, string name, string description, IfcLocalPlacement placement)
    {
        return model.Instances.New<IfcDistributionPort>(p =>
        {
            p.Name = name;
            p.Description = description;
            p.ObjectPlacement = placement;
            p.FlowDirection = IfcFlowDirectionEnum.NOTDEFINED;
            p.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
        });
    }

    #endregion
}