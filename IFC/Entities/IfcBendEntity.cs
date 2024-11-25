using IFC_Converter.Math;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC_Converter.IFC.Entities;

public class IfcBendEntity : IfcAbstractEntity
{
    private readonly StartBendEntity _startBendEntity;
    private readonly StartNodeEntity _startNodeEntity;
    private readonly StartPipeEntity[] _startPipeEntities;

    public Vector3 Coordinates;
    
    public IfcBendEntity(StartBendEntity startBendEntity, StartNodeEntity startNodeEntity, StartPipeEntity[] startPipeEntities)
    {
        _startBendEntity = startBendEntity;
        _startNodeEntity = startNodeEntity;
        _startPipeEntities = startPipeEntities;

        Coordinates = startNodeEntity.GetCoordinates();
    }
    
    public override void CreateAndAdd(IModel model)
    {
        IfcPipeSegment bendSegment = model.Instances.New<IfcPipeSegment>(s =>
        {
            s.Name = _startBendEntity.GetName();
            s.ObjectPlacement = CreateLocalPlacementAndDirection(model, _startPipeEntities[0].GetCoordinates(), _startPipeEntities[0].GetDirection());
            s.Representation = CreateBendShape(model);
        });
        AddProperties(model, bendSegment, _startBendEntity);
    }

    private IfcProductDefinitionShape CreateBendShape(IModel model)
    {
        IfcCircleProfileDef profileDef = model.Instances.New<IfcCircleProfileDef>(c =>
        {
            c.ProfileType = IfcProfileTypeEnum.AREA;
            c.Radius = _startPipeEntities[0].GetOutsideDiameter() / 2;
        });
        
        Vector3 firstCoord = Coordinates + _startPipeEntities[0].GetDirection() / 3;
        IfcCartesianPoint firstPoint = model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(
            firstCoord.x, firstCoord.y, firstCoord.z
        ));
        
        Vector3 secondCoord = Coordinates + _startPipeEntities[1].GetDirection() / 3;
        IfcCartesianPoint secondPoint = model.Instances.New<IfcCartesianPoint>(p => p.SetXYZ(
            secondCoord.x, secondCoord.y, secondCoord.z
        ));

        IfcPolyline polyline = model.Instances.New<IfcPolyline>(l =>
        {
            l.Points.Add(firstPoint);
            l.Points.Add(secondPoint);
        });
        
        IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(s =>
        {
            s.SweptArea = profileDef;
            s.Directrix = polyline;
            s.StartParam = 0;
            s.EndParam = 1;
        });

        IfcShapeRepresentation representation = CreateShapeRepresentation(model, sweptAreaSolid);
        IfcProductDefinitionShape productDefinitionShape = model.Instances.New<IfcProductDefinitionShape>(repr => repr.Representations.Add(representation));

        return productDefinitionShape;
    }
}