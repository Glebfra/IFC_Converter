using IFC_Converter.IFC.Entities.Abstract;
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

namespace IFC_Converter.IFC.Entities;

public class IfcMilterJointEntity : IfcAbstractEntity
{
    private readonly StartMilterJointEntity _startMilterJointEntity;
    private readonly IfcNodeEntity _ifcNodeEntity;
    private readonly IfcPipeEntity[] _ifcPipeEntities;

    private IfcPipeFitting _pipeFitting;

    private double _pipeAngle;

    public double Length => 2 * Depth;

    public sealed override XbimMatrix3D ObjectMatrix3D { get; protected set; }
    public XbimVector3D[] PipesDirection { get; }
    public XbimVector3D[] DirectionToPipes { get; }
    
    public double Depth { get; }

    public IfcMilterJointEntity(StartMilterJointEntity startMilterJointEntity, IfcNodeEntity ifcNodeEntity, IfcPipeEntity[] ifcPipeEntities)
    {
        _startMilterJointEntity = startMilterJointEntity;
        _ifcNodeEntity = ifcNodeEntity;
        _ifcPipeEntities = ifcPipeEntities;
        _ifcNodeEntity.connEntities.Add(this);
        
        XbimVector3D coordinates = _ifcNodeEntity.ObjectMatrix3D.Translation;
        PipesDirection = ifcPipeEntities.Select(pipe => pipe.ObjectMatrix3D.Forward).ToArray();
        DirectionToPipes = ifcPipeEntities.Select(pipe => IfcAxis.GetDirectionToPipe(pipe, coordinates)).ToArray();
        
        XbimVector3D upDirection = XbimVector3D.CrossProduct(DirectionToPipes[0] * -1, DirectionToPipes[1]).Normalized();
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, DirectionToPipes[0] * -1, upDirection);

        Depth = Math.Min(ifcPipeEntities[0].Depth, ifcPipeEntities[1].Depth) * 0.1;
        _pipeAngle = PipesDirection[0].Angle(PipesDirection[1]);
    }
    
    public override IfcProduct CreateAndAdd(IModel model)
    {
        IfcDirection upDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Up);
        
        IfcCartesianPoint objectPoint = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        IfcAxis2Placement3D objectAxis = IfcAxis.CreateAxis2Placement3D(model, objectPoint);
        IfcLocalPlacement objectPlacement = IfcAxis.CreateLocalPlacement(model, objectAxis);

        IfcRepresentationItem[] ifcRepresentationItems = new IfcRepresentationItem[_ifcPipeEntities.Length + 1];
        for (int i = 0; i < _ifcPipeEntities.Length; i++)
        {
            ifcRepresentationItems[i] = CreateExtrudedAreaSolid(model, _ifcPipeEntities[i], 0);
            _ifcPipeEntities[i].Clip(_ifcNodeEntity, Depth);
        }

        ifcRepresentationItems[_ifcPipeEntities.Length] = model.Instances.New<IfcBooleanResult>(result =>
        {
            result.Operator = IfcBooleanOperator.INTERSECTION;
            result.FirstOperand = CreateExtrudedAreaSolid(model, _ifcPipeEntities[0], Depth);
            result.SecondOperand = CreateExtrudedAreaSolid(model, _ifcPipeEntities[1], Depth);
        });

        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, ifcRepresentationItems);
        IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        _pipeFitting = CreateMilterJoint(model, shape, objectPlacement);
        ConnectPorts(model);
        AddProperties(model);

        return _pipeFitting;
    }

    private IfcPipeFitting CreateMilterJoint(IModel model, IfcProductDefinitionShape shape, IfcLocalPlacement placement)
    {
        return model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.ObjectPlacement = placement;
            fitting.Name = _startMilterJointEntity.GetName();
            fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
            fitting.Representation = shape;
        });
    }

    private IfcExtrudedAreaSolid CreateExtrudedAreaSolid(IModel model, IfcPipeEntity ifcPipeEntity, double displacement)
    {
        XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(ifcPipeEntity, ObjectMatrix3D.Translation).Normalized();
        XbimVector3D localUp = ObjectMatrix3D.Up;
        
        IfcCartesianPoint point = IfcAxis.CreatePoint(model, XbimVector3D.Zero - directionToPipe * displacement);
        IfcDirection extrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));

        IfcDirection ifcDirectionToPipe = IfcAxis.CreateDirection(model, directionToPipe);
        IfcDirection ifcLocalUp = IfcAxis.CreateDirection(model, localUp);
        IfcAxis2Placement3D placement3D = IfcAxis.CreateAxis2Placement3D(model, point, ifcDirectionToPipe, ifcLocalUp);
        
        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(
            model,
            ifcPipeEntity.Diameter / 2,
            XbimVector3D.Zero,
            new XbimVector3D(1, 0, 0)
        );

        return model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.ExtrudedDirection = extrudedDirection;
            solid.Depth = Depth;
            solid.Position = placement3D;
        });
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
    
    private void AddProperties(IModel model)
    {
        #region Pset_PipeFittingTypeStart

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeStart";
                foreach (var kvp in _startMilterJointEntity.GetData())
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
                    length.LengthValue = Length;
                    length.Formula = "radius*angle; [angle]=rad, [radius]=metre";
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                {
                    weight.Name = "NetWeight";
                    weight.WeightValue = ValueConverter.ValueConverter.TfToKg(_startMilterJointEntity.GetWeight()) * Length;
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