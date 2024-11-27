using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
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

    public XbimVector3D Coordinates;
    
    public IfcBendEntity(StartBendEntity startBendEntity, StartNodeEntity startNodeEntity, StartPipeEntity[] startPipeEntities)
    {
        _startBendEntity = startBendEntity;
        _startPipeEntities = startPipeEntities;

        Coordinates = startNodeEntity.GetCoordinates();
    }
    
    public override IfcObject CreateAndAdd(IModel model)
    {
        XbimVector3D firstPipeDirection = GetRightPipeDirection(_startPipeEntities[0], Coordinates).Normalized();
        XbimVector3D secondPipeDirection = GetRightPipeDirection(_startPipeEntities[1], Coordinates).Normalized();

        XbimVector3D startBendCoordinates = Coordinates + firstPipeDirection * _startBendEntity.GetRadius();
        XbimVector3D endBendCoordinates = Coordinates + secondPipeDirection * _startBendEntity.GetRadius();
        XbimVector3D circleCenter = (firstPipeDirection + secondPipeDirection) * _startBendEntity.GetRadius();
        
        IfcCircle circle = CreateCircle(model, _startBendEntity.GetRadius(), XbimVector3D.Zero, new XbimVector3D(0, 0, 1));

        double pipeAngle = firstPipeDirection.Angle(secondPipeDirection);
        IfcTrimmedCurve trimmedCurve = CreateTrimmedCurve(model, circle, 0, pipeAngle);
        IfcPlane plane = CreatePlane(model, Coordinates, new XbimVector3D(0, 0, 1));

        IfcCircleProfileDef profileDef = CreateCircleProfileDef(
            model,
            _startPipeEntities[0].GetOutsideDiameter() / 2 * 1.1,
            XbimVector3D.Zero,
            new XbimVector3D(1, 0, 0)
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