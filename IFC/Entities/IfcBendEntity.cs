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

    private readonly XbimMatrix3D _objectWorldMatrix;
    private readonly XbimVector3D[] _pipesDirection;
    private readonly XbimVector3D[] _directionToPipes;
    
    public IfcBendEntity(StartBendEntity startBendEntity, StartNodeEntity startNodeEntity, StartPipeEntity[] startPipeEntities)
    {
        _startBendEntity = startBendEntity;
        _startPipeEntities = startPipeEntities;
        
        XbimVector3D coordinates = startNodeEntity.GetCoordinates();
        _pipesDirection = startPipeEntities.Select(pipe => pipe.GetDirection().Normalized()).ToArray();
        _directionToPipes = startPipeEntities.Select(pipe => GetDirectionToPipe(pipe, coordinates)).ToArray();

        XbimVector3D upDirection = XbimVector3D.CrossProduct(_pipesDirection[0], _pipesDirection[1]).Normalized();
        _objectWorldMatrix = XbimMatrix3D.CreateWorld(coordinates, _directionToPipes[0] * -1, upDirection);
    }
    
    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = CreateBendShape(model);
        IfcShapeRepresentation shapeRepresentation = CreateShapeRepresentation(model, sweptAreaSolid);
        IfcProductDefinitionShape shape = CreateProductDefinitionShape(model, shapeRepresentation);
        IfcPipeSegment pipe = CreatePipeSegment(model, _startBendEntity.GetName(), CreateLocalPlacement(model, _objectWorldMatrix.Translation), shape);
        AddProperties(model, pipe, _startBendEntity);

        return pipe;
    }

    private IfcSurfaceCurveSweptAreaSolid CreateBendShape(IModel model)
    {
        double pipeAngle = _pipesDirection[0].Angle(_pipesDirection[1]);
        XbimVector3D circleCenter = CalculateCircleCenter(pipeAngle);
        
        IfcCircle circle = CreateCircle(model, _startBendEntity.GetRadius(), circleCenter, _objectWorldMatrix.Up, _objectWorldMatrix.Right);
        IfcTrimmedCurve trimmedCurve = CreateTrimmedCurve(model, circle, 0, pipeAngle);
        IfcPlane plane = CreatePlane(model, _objectWorldMatrix.Translation, _objectWorldMatrix.Up);

        IfcCircleProfileDef profileDef = CreateCircleProfileDef(
            model,
            _startPipeEntities[0].GetOutsideDiameter() / 2 * 1.1,
            XbimVector3D.Zero,
            new XbimVector3D(1, 0, 0)
        );

        return model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.Directrix = trimmedCurve;
            solid.ReferenceSurface = plane;
        });
    }

    private XbimVector3D CalculateCircleCenter(double pipeAngle)
    {
        XbimVector3D dirToCenter = (_directionToPipes[0].Normalized() + _directionToPipes[1].Normalized()).Normalized();
        double lengthToCenter = _startBendEntity.GetRadius() / Math.Cos(pipeAngle / 2);
        return dirToCenter * lengthToCenter;
    }
}