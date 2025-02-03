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
using Xbim.Ifc4.RepresentationResource;

namespace IFC_Converter.IFC.Entities;

public class IfcReducerConcentricEntity : IfcAbstractEntity
{
    private readonly StartReducerConcentricEntity _startReducerConcentric;
    private readonly IfcPipeEntity[] _pipeEntities;
    private readonly IfcNodeEntity _nodeEntity;

    private IfcPipeFitting? _pipeFitting;

    public readonly XbimMatrix3D ObjectMatrix3D;
    public readonly double Length;

    public IfcCartesianPoint? Location { get; private set; }
    public IfcDirection? Axis { get; private set; }
    public IfcDirection? RefDirection { get; private set; }
    
    public IfcReducerConcentricEntity(StartReducerConcentricEntity startReducerConcentric, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities)
    {
        _startReducerConcentric = startReducerConcentric;
        _nodeEntity = nodeEntity;
        _pipeEntities = pipeEntities;

        XbimVector3D coordinates = nodeEntity.ObjectMatrix3D.Translation;
        XbimVector3D directionToPipe = IfcAxis.GetDirectionToPipe(pipeEntities[1], coordinates);
        
        XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
        XbimVector3D forward = directionToPipe.Normalized();
        if (forward == WorldUp || forward == -1 * WorldUp)
            WorldUp = new XbimVector3D(0, 1, 0);
        XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp).Normalized();

        ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, WorldUp);
        Length = _startReducerConcentric.GetLengthOfConicalPart();
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        Location = IfcAxis.CreatePoint(model, ObjectMatrix3D.Translation);
        Axis = IfcAxis.CreateDirection(model, ObjectMatrix3D.Forward);
        RefDirection = IfcAxis.CreateDirection(model, ObjectMatrix3D.Right);

        IfcCartesianPoint connShapePoint = IfcAxis.CreatePoint(model, XbimVector3D.Zero);
        IfcDirection connShapeAxis = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
        IfcAxis1Placement connShapePlacement = IfcAxis.CreateAxis1Placement(model, connShapePoint, connShapeAxis);
        IfcAxis2Placement3D objectPlacement3D = IfcAxis.CreateAxis2Placement3D(model, Location, Axis, RefDirection);
        IfcLocalPlacement objectPlacement = IfcAxis.CreateLocalPlacement(model, objectPlacement3D);

        IfcPolyline polyline = CreateTrapezoid(model);
        IfcArbitraryClosedProfileDef profileDef = IfcGeometry.CreateProfile(model, polyline);
        IfcRevolvedAreaSolid coneShape = model.Instances.New<IfcRevolvedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.Axis = connShapePlacement;
            solid.Angle = Math.PI * 2;
        });

        IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, coneShape);
        IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
        _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
        {
            fitting.Name = _startReducerConcentric.GetName();
            fitting.Representation = shape;
            fitting.ObjectPlacement = objectPlacement;
            fitting.Tag = "Reducer";
            fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
        });

        AddProperties(model);
        _pipeEntities[1].Clip(_nodeEntity, Length);

        return _pipeFitting;
    }

    private IfcPolyline CreateTrapezoid(IModel model)
    {
        double startRadius = _pipeEntities[0].Diameter / 2;
        double endRadius = _pipeEntities[1].Diameter / 2;
        
        return model.Instances.New<IfcPolyline>(polyline =>
        {
            polyline.Points.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0)));
            polyline.Points.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(startRadius, 0, 0)));
            polyline.Points.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(endRadius, 0, Length)));
            polyline.Points.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, Length)));
            polyline.Points.Add(model.Instances.New<IfcCartesianPoint>(point => point.SetXYZ(0, 0, 0)));
        });
    }

    protected IfcRelConnectsPorts ConnectPorts(IModel model)
    {
        var closestPorts = (
            from port in _pipeEntities.SelectMany(pipe => pipe.Ports)
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
        if (_pipeFitting == null)
            throw new Exception("The required field is null. First call the CreateAndAdd method");
        
        #region Pset_PipeFittingTypeStart
        
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeFitting);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeFittingTypeStart";
                foreach (var kvp in _startReducerConcentric.GetData())
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