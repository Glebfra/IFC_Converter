using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using IFC_Converter.Math;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public sealed class IfcPipeEntity : IfcAbstractEntity
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

    public override void CreateAndAdd(IModel model)
    {
        IfcLocalPlacement localStartPlacement = CreateLocalPlacementAndDirection(model, StartCoordinates, Direction);
        IfcLocalPlacement localEndPlacement = CreateLocalPlacementAndDirection(model, EndCoordinates, Direction);

        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        IfcPipeSegment? pipeSegment = model.Instances.New<IfcPipeSegment>(p =>
        {
            p.Name = _pipeEntity.GetName();
            p.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            p.ObjectPlacement = localStartPlacement;
            p.Representation = productDefShape;
        });
        AddProperties(model, pipeSegment, _pipeEntity);

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

        IfcShapeRepresentation? shapeRep = CreateShapeRepresentation(model, extrudedSolid);
        IfcProductDefinitionShape? productDefShape = model.Instances.New<IfcProductDefinitionShape>(repr => repr.Representations.Add(shapeRep));

        return productDefShape;
    }
}