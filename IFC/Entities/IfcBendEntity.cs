using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace IFC_Converter.IFC.Entities;

public class IfcBendEntity : IfcAbstractEntity
{
    private readonly StartBendEntity _startBendEntity;
    private readonly IfcNodeEntity _ifcNodeEntity;
    private readonly IfcPipeEntity[] _ifcPipeEntities;

    public XbimMatrix3D ObjectWorldMatrix { get; }
    public XbimVector3D[] PipesDirection { get; }
    public XbimVector3D[] DirectionToPipes { get; }

    public IfcBendEntity(StartBendEntity startBendEntity, IfcNodeEntity ifcNodeEntity, IfcPipeEntity[] ifcPipeEntities)
    {
        _startBendEntity = startBendEntity;
        _ifcPipeEntities = ifcPipeEntities;
        _ifcNodeEntity = ifcNodeEntity;
        
        XbimVector3D coordinates = _ifcNodeEntity.Coordinates;
        PipesDirection = ifcPipeEntities.Select(pipe => pipe.ObjectMatrix3D.Forward.Normalized()).ToArray();
        DirectionToPipes = ifcPipeEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();

        XbimVector3D upDirection = XbimVector3D.CrossProduct(PipesDirection[0], PipesDirection[1]).Normalized();
        ObjectWorldMatrix = XbimMatrix3D.CreateWorld(coordinates, DirectionToPipes[0] * -1, upDirection);
    }
    
    public override IfcObject CreateAndAdd(IModel model)
    {
        IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = CreateBendShape(model);
        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, sweptAreaSolid);
        IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        IfcPipeSegment pipe = IfcSegment.CreatePipeSegment(model, _startBendEntity.GetName(), IfcAxis.CreateLocalPlacement(model, ObjectWorldMatrix.Translation), shape);
        IfcProperty.AddProperties(model, pipe, _startBendEntity.GetData());

        double pipeAngle = PipesDirection[0].Angle(PipesDirection[1]);
        double clipLength = _startBendEntity.GetRadius() * Math.Tan(pipeAngle / 2);
        foreach (var ifcPipeEntity in _ifcPipeEntities)
        {
            ifcPipeEntity.Clip(model, _ifcNodeEntity, clipLength);
        }

        return pipe;
    }

    private IfcSurfaceCurveSweptAreaSolid CreateBendShape(IModel model)
    {
        double pipeAngle = PipesDirection[0].Angle(PipesDirection[1]);
        XbimVector3D circleCenter = CalculateCircleCenter(pipeAngle);
        
        IfcCircle circle = IfcGeometry.CreateCircle(model, _startBendEntity.GetRadius(), circleCenter, ObjectWorldMatrix.Up, ObjectWorldMatrix.Right);
        IfcTrimmedCurve trimmedCurve = IfcGeometry.CreateTrimmedCurve(model, circle, 0, pipeAngle);
        IfcPlane plane = IfcGeometry.CreatePlane(model, ObjectWorldMatrix.Translation, ObjectWorldMatrix.Up);

        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(
            model,
            _ifcPipeEntities[0].Diameter / 2,
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
        XbimVector3D dirToCenter = (DirectionToPipes[0].Normalized() + DirectionToPipes[1].Normalized()).Normalized();
        double lengthToCenter = _startBendEntity.GetRadius() / Math.Cos(pipeAngle / 2);
        return dirToCenter * lengthToCenter;
    }
}