using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc.Extensions;
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
using Xbim.Ifc4.QuantityResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC_Converter.IFC.Entities;

public class IfcBendEntity : IfcAbstractEntity
{
    private readonly StartBendEntity _startBendEntity;
    private readonly IfcNodeEntity _ifcNodeEntity;
    private readonly IfcPipeEntity[] _ifcPipeEntities;

    private IfcPipeFitting _pipeFitting;
    
    private readonly double _pipeAngle;

    public XbimMatrix3D ObjectMatrix3D { get; }
    public XbimVector3D[] PipesDirection { get; }
    public XbimVector3D[] DirectionToPipes { get; }

    public IfcBendEntity(StartBendEntity startBendEntity, IfcNodeEntity ifcNodeEntity, IfcPipeEntity[] ifcPipeEntities)
    {
        _startBendEntity = startBendEntity;
        _ifcPipeEntities = ifcPipeEntities;
        _ifcNodeEntity = ifcNodeEntity;

        XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
        PipesDirection = ifcPipeEntities.Select(pipe => pipe.ObjectMatrix3D.Forward).ToArray();
        DirectionToPipes = ifcPipeEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();

        XbimVector3D upDirection = XbimVector3D.CrossProduct(DirectionToPipes[0] * -1, DirectionToPipes[1]).Normalized();
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, DirectionToPipes[0] * -1, upDirection);
        
        _pipeAngle = PipesDirection[0].Angle(PipesDirection[1]);
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        IfcSurfaceCurveSweptAreaSolid sweptAreaSolid = CreateBendShape(model);
        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, sweptAreaSolid);
        IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        _pipeFitting = CreateBend(model, shape);
        AddProperties(model);
        ClipConnectedPipes();
        ConnectPorts(model);

        return _pipeFitting;
    }

    private IfcRelConnectsPorts ConnectPorts(IModel model)
    {
        var closestPorts = (
            from port in _ifcPipeEntities.SelectMany(pipe => pipe.Ports)
            let distance = (port.ObjectPlacement.ToMatrix3D().Translation - ObjectMatrix3D.Translation).Length
            orderby distance
            select port
        ).Take(2).ToArray();

        return model.Instances.New<IfcRelConnectsPorts>(ports =>
        {
            ports.Name = $"{closestPorts[0].GlobalId}|{closestPorts[1].GlobalId}";
            ports.Description = "Flow";
            ports.RelatingPort = closestPorts[0];
            ports.RelatedPort = closestPorts[1];
            ports.RealizingElement = _pipeFitting;
        });
    }

    private IfcPipeFitting CreateBend(IModel model, IfcProductDefinitionShape shape)
    {
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        IfcAxis2Placement3D axis2Placement3D = IfcAxis.CreateAxis2Placement3D(model, point);
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, axis2Placement3D);
        
        return model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.Name = _startBendEntity.GetName();
            fitting.Tag = "Elbow";
            fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
            fitting.Representation = shape;
            fitting.ObjectPlacement = localPlacement;
        });
    }

    private IfcSurfaceCurveSweptAreaSolid CreateBendShape(IModel model)
    {
        XbimVector3D circleCenter = CalculateCircleCenter();
        
        IfcCircle circle = IfcGeometry.CreateCircle(model, _startBendEntity.GetRadius(), circleCenter, ObjectMatrix3D.Up, ObjectMatrix3D.Right);
        IfcTrimmedCurve trimmedCurve = IfcGeometry.CreateTrimmedCurve(model, circle, 0, _pipeAngle);
        IfcPlane plane = IfcGeometry.CreatePlane(model, ObjectMatrix3D.Translation, ObjectMatrix3D.Up);
        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, _ifcPipeEntities[0].Diameter / 2, XbimVector3D.Zero, new XbimVector3D(1, 0, 0));

        return model.Instances.New<IfcSurfaceCurveSweptAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.Directrix = trimmedCurve;
            solid.ReferenceSurface = plane;
        });
    }

    private XbimVector3D CalculateCircleCenter()
    {
        XbimVector3D dirToCenter = (DirectionToPipes[0].Normalized() + DirectionToPipes[1].Normalized()).Normalized();
        double lengthToCenter = _startBendEntity.GetRadius() / Math.Cos(_pipeAngle / 2);
        return dirToCenter * lengthToCenter;
    }

    private void ClipConnectedPipes()
    {
        double clipLength = _startBendEntity.GetRadius() * Math.Tan(_pipeAngle / 2);
        foreach (var ifcPipeEntity in _ifcPipeEntities)
        {
            ifcPipeEntity.Clip(_ifcNodeEntity, clipLength);
        }
    }

    private XbimVector3D CalculateAlternateCircleCenter()
    {
        double lengthToCenter = _startBendEntity.GetRadius() * Math.Tan(_pipeAngle / 2);
        XbimVector3D dirToCenter = new XbimVector3D(-1, 0, 0);
        
        return dirToCenter * lengthToCenter;
    }

    private IfcRevolvedAreaSolid CreateAlternateBendShape(IModel model)
    {
        XbimVector3D circleCenter = CalculateAlternateCircleCenter();

        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(
            model,
            _ifcPipeEntities[0].Diameter / 2,
            XbimVector3D.Zero,
            new XbimVector3D(1, 0, 0)
        );
        
        double lengthToCenter = _startBendEntity.GetRadius() * Math.Tan(_pipeAngle / 2);

        IfcRevolvedAreaSolid sweptAreaSolid = model.Instances.New<IfcRevolvedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.Axis = IfcAxis.CreateAxis1Placement(model, circleCenter, new XbimVector3D(0, -1, 0));
            solid.Angle = new IfcPlaneAngleMeasure(_pipeAngle);
            solid.Position = IfcAxis.CreateAxis2Placement3D(model, DirectionToPipes[0] * lengthToCenter, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
        });

        return sweptAreaSolid;
    }

    private void AddProperties(IModel model)
    {
        #region Pset_PipeFittingTypeStart

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeStart";
                foreach (var kvp in _startBendEntity.GetData())
                {
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = kvp.Key;
                        value.NominalValue = new IfcText(kvp.Value);
                    }));
                }
            });
        });

        #endregion

        #region Qto_PipeFittingBaseQuantities

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
            {
                quantity.Name = "Qto_PipeFittingBaseQuantities";
                quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                {
                    length.Name = "Length";
                    length.LengthValue = _pipeAngle * _startBendEntity.GetRadius();
                    length.Formula = "radius*angle; [angle]=rad, [radius]=metre";
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                {
                    weight.Name = "NetWeight";
                    weight.WeightValue = _startBendEntity.GetWeight();
                }));
            });
        });

        #endregion
        
        #region DEBUG

        #if DEBUG
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Debug Properties";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Coordinates";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Translation.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Forward direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Forward.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Right direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Right.ToString());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "Up direction";
                    value.NominalValue = new IfcText(ObjectMatrix3D.Up.ToString());
                }));
            });
        });
        #endif

        #endregion
    }
}