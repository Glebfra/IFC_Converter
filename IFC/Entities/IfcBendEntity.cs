using IFC_Converter.Math;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC_Converter.IFC.Entities;

public class IfcBendEntity : IfcAbstractEntity
{
    private readonly StartBendEntity _startBendEntity;
    private readonly StartPipeEntity[] _startPipeEntities;

    public Vector3 Coordinates;
    
    public IfcBendEntity(StartBendEntity startBendEntity, StartNodeEntity startNodeEntity, StartPipeEntity[] startPipeEntities)
    {
        _startBendEntity = startBendEntity;
        _startPipeEntities = startPipeEntities;

        Coordinates = startNodeEntity.GetCoordinates();
    }
    
    public override IfcObject CreateAndAdd(IModel model)
    {
        Vector3 firstPipeDirection = GetRightPipeDirection(_startPipeEntities[0], Coordinates).Normalized;
        Vector3 secondPipeDirection = GetRightPipeDirection(_startPipeEntities[1], Coordinates).Normalized;

        Vector3 startBendCoordinates = Coordinates + firstPipeDirection * _startBendEntity.GetRadius();
        Vector3 endBendCoordinates = Coordinates + secondPipeDirection * _startBendEntity.GetRadius();
        Vector3 circleCenter = (firstPipeDirection + secondPipeDirection) * _startBendEntity.GetRadius();
        
        IfcCircle circle = CreateCircle(model, _startBendEntity.GetRadius(), Vector3.Zero, Vector3.Up);

        double angle = Vector3.Angle(firstPipeDirection, secondPipeDirection);
        IfcTrimmedCurve trimmedCurve = CreateTrimmedCurve(model, circle, 0, angle);
        IfcPlane plane = CreatePlane(model, Coordinates, Vector3.Up);

        IfcCircleProfileDef profileDef = CreateCircleProfileDef(
            model,
            _startPipeEntities[0].GetOutsideDiameter() / 2 * 1.1,
            Vector3.Zero,
            Vector3.Forward
        );

        IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.Directrix = trimmedCurve;
            solid.ReferenceSurface = plane;
        });

        IfcShapeRepresentation shapeRepresentation = CreateShapeRepresentation(model, sweptAreaSolid);
        IfcProductDefinitionShape shape = CreateProductDefinitionShape(model, shapeRepresentation);
        IfcPipeSegment pipe = CreatePipeSegment(model, _startBendEntity.GetName(), CreateLocalPlacement(model, Coordinates + circleCenter), shape);
        AddProperties(model, pipe, _startBendEntity);

        return pipe;
    }
}