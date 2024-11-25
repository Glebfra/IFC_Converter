using IFC_Converter.Math;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public abstract class IfcAbstractEntity
{
    public abstract void CreateAndAdd(IModel model);

    protected static IfcCartesianPoint CreatePoint(IModel model, Vector3 coordinates)
    {
        return model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(coordinates.x, coordinates.y, coordinates.z));
    }
    
    protected static IfcDirection CreateDirection(IModel model, Vector3 direction)
    {
        return model.Instances.New<IfcDirection>(d => d.SetXYZ(direction.x, direction.y, direction.z));
    }

    protected static IfcRelDefinesByProperties AddProperties(IModel model, IfcObject ifcObject,
        StartAbstractEntity entity)
    {
        IfcRelDefinesByProperties properties = model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(ifcObject);
            rel.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pipe properties";
                foreach (var kvp in entity.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(prop =>
                    {
                        prop.Name = kvp.Key;
                        prop.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });

        return properties;
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
    
    protected static IfcDistributionPort AddPort(IModel model, IfcLocalPlacement localPlacement)
    {
        IfcDistributionPort port = model.Instances.New<IfcDistributionPort>(p =>
        {
            p.Name = "Input Port";
            p.Description = "Description for input port";
            p.ObjectPlacement = localPlacement;
            p.FlowDirection = IfcFlowDirectionEnum.NOTDEFINED;
            p.PredefinedType = IfcDistributionPortTypeEnum.PIPE;
        });

        return port;
    }

    protected static IfcShapeRepresentation CreateShapeRepresentation(IModel model, IfcRepresentationItem representationItem)
    {
        IfcShapeRepresentation? shapeRep = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(representationItem);
        });

        return shapeRep;
    }
}