using IFC_Converter.IFC.Entities.Abstract;
using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
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

namespace IFC_Converter.IFC.Entities;

public class IfcReducerConcentricEntity : IfcAbstractReducerEntity
{
    protected override IfcPipeFitting? _pipeFitting { get; set; }

    public IfcCartesianPoint? Location { get; private set; }
    public IfcDirection? Axis { get; private set; }
    public IfcDirection? RefDirection { get; private set; }
    
    public IfcReducerConcentricEntity(StartReducerConcentricEntity startReducerConcentric, IfcNodeEntity nodeEntity, IfcPipeEntity[] pipeEntities) 
        : base(startReducerConcentric, nodeEntity, pipeEntities)
    {
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
            fitting.Name = _startReducer.GetName();
            fitting.Representation = shape;
            fitting.ObjectPlacement = objectPlacement;
            fitting.Tag = "Reducer";
            fitting.PredefinedType = IfcPipeFittingTypeEnum.TRANSITION;
        });
        _pipeEntities[1].Clip(_nodeEntity, Length);
        
        AddProperties(model);
        ConnectPorts(model);

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
}