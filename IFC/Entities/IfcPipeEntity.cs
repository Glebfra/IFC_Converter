using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.RepresentationResource;
using IFC_Converter.Math;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public sealed class IfcPipeEntity
{
    private readonly StartPipeEntity _pipeEntity;

    public Vector3 StartCoordinates;
    public Vector3 EndCoordinates;
    public Vector3 Direction;
    public double Diameter;

    public IfcPipeEntity(StartPipeEntity pipeEntity)
    {
        _pipeEntity = pipeEntity;
        StartCoordinates = _pipeEntity.GetCoordinates();
        Direction = _pipeEntity.GetDirection();
        EndCoordinates = StartCoordinates + Direction;
        Diameter = _pipeEntity.GetOutsideDiameter();
    }

    public IfcPipeSegment CreateAndAddPipe(IModel model)
    {
        IfcLocalPlacement localStartPlacement = CreateLocalPlacement(model, StartCoordinates, Direction);
        IfcLocalPlacement localEndPlacement = CreateLocalPlacement(model, EndCoordinates, Direction);

        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        IfcPipeSegment? pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = _pipeEntity.GetName();
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = localStartPlacement;
            p.Representation = productDefShape;
        });
        AddProperties(model, pipeSegment);

        IfcDistributionPort origin = AddPort(model, localStartPlacement);
        IfcDistributionPort destination = AddPort(model, localEndPlacement);
        
        model.Instances.New<IfcRelNests>(rel =>
        {
            rel.Name = "Pipe Ports";
            rel.Description = "Connects two ports of pipe";
            rel.RelatingObject = pipeSegment;
            rel.RelatedObjects.Add(origin);
            rel.RelatedObjects.Add(destination);
        });

        return pipeSegment;
    }

    private static IfcLocalPlacement CreateLocalPlacement(IModel model, Vector3 coordinates, Vector3 direction)
    {
        IfcLocalPlacement? localStartPlacement = model.Instances.New<IfcLocalPlacement>(lp =>
        {
            lp.RelativePlacement = model.Instances.New<IfcAxis2Placement3D>(pos =>
            {
                pos.Location = model.Instances.New<IfcCartesianPoint>(pt => pt.SetXYZ(
                    coordinates.x, coordinates.y, coordinates.z
                ));
                pos.Axis = model.Instances.New<IfcDirection>(dir => dir.SetXYZ(
                    direction.x, direction.y, direction.z
                ));
            });
        });
        
        return localStartPlacement;
    }

    private static IfcDistributionPort AddPort(IModel model, IfcLocalPlacement localPlacement)
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

    private IfcProductDefinitionShape CreatePipeShape(IModel model)
    {
        IfcCircleProfileDef? profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = Diameter / 2;
        });

        IfcExtrudedAreaSolid? extrudedSolid = model.Instances.New<IfcExtrudedAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.ExtrudedDirection = model.Instances.New<IfcDirection>(d => d.SetXYZ(0, 0, 1));
            s.Depth = Direction.Length();
        });

        IfcShapeRepresentation? shapeRep = model.Instances.New<IfcShapeRepresentation>(sr =>
        {
            sr.ContextOfItems = model.Instances.OfType<IfcGeometricRepresentationContext>().FirstOrDefault();
            sr.RepresentationIdentifier = "Body";
            sr.RepresentationType = "SweptSolid";
            sr.Items.Add(extrudedSolid);
        });

        IfcProductDefinitionShape? productDefShape = model.Instances.New<IfcProductDefinitionShape>(repr => { repr.Representations.Add(shapeRep); });

        return productDefShape;
    }

    private IfcRelDefinesByProperties AddProperties(IModel model, IfcPipeSegment pipeSegment)
    {
        IfcRelDefinesByProperties properties = model.Instances.New<IfcRelDefinesByProperties>(rel =>
        {
            rel.RelatedObjects.Add(pipeSegment);
            rel.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pipe properties";
                foreach (var kvp in _pipeEntity.GetData())
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
}