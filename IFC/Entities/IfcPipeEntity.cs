using IFC_Converter.IFC.Tools;
using IFC_Converter.Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.GeometricConstraintResource;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;

namespace IFC_Converter.IFC.Entities;

public class IfcPipeEntity : IfcAbstractEntity
{
    public StartPipeEntity PipeEntity { get; }
    public XbimMatrix3D ObjectMatrix3D { get; }
    public double Diameter { get; }

    public double Depth { get; private set; }

    private IfcNodeEntity[] _nodeEntities;
    private IfcPipeSegment _pipeSegment;
    
    private delegate void DepthChanged(double newDepth);
    private delegate void CoordinatesChanged(XbimVector3D newCoordinates);
    
    private event DepthChanged _depthChanged;
    private event CoordinatesChanged _coordinatesChanged;

    public IfcPipeEntity(StartPipeEntity pipeEntity, IfcNodeEntity[] ifcNodeEntities)
    {
        PipeEntity = pipeEntity;
        _nodeEntities = ifcNodeEntities;

        XbimVector3D direction = PipeEntity.GetDirection();
        Diameter = PipeEntity.GetOutsideDiameter();
        
        Depth = direction.Length;
        _depthChanged += depth => Depth = depth;

        XbimVector3D WorldUp = new XbimVector3D(0, 0, 1);
        XbimVector3D forward = direction.Normalized();
        if (forward == WorldUp || forward == -1 * WorldUp) 
            WorldUp = new XbimVector3D(0, 1, 0);
        XbimVector3D up = XbimVector3D.CrossProduct(forward, WorldUp);
        
        ObjectMatrix3D = XbimMatrix3D.CreateWorld(PipeEntity.GetCoordinates(), forward, up);
    }

    public override IfcProduct CreateAndAdd(IModel model)
    {
        IfcProductDefinitionShape productDefShape = CreatePipeShape(model);
        _pipeSegment = CreatePipe(model, productDefShape);
        AddProperties(model);

        model.Instances.New<IfcRelNests>(nests =>
        {
            nests.Name = "Pipe ports";
            nests.Description = "Connects two ports of the pipe";
            nests.RelatingObject = _pipeSegment;
            nests.RelatedObjects.AddRange(_nodeEntities.Select(nodeEntity => nodeEntity.Port));
        });

        return _pipeSegment;
    }

    public void Clip(IfcNodeEntity nodeEntity, double clipLength)
    {
        if (IsStartNode(nodeEntity))
            _coordinatesChanged?.Invoke(ObjectMatrix3D.Forward * clipLength);
        _depthChanged?.Invoke(Depth - clipLength);
    }

    private bool IsStartNode(IfcNodeEntity nodeEntity)
    {
        XbimVector3D nodeCoordinates = nodeEntity.Coordinates;
        XbimVector3D startPipeCoordinates = ObjectMatrix3D.Translation;
        XbimVector3D endPipeCoordinates = ObjectMatrix3D.Translation + ObjectMatrix3D.Forward * Depth;

        return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
    }

    private IfcProductDefinitionShape CreatePipeShape(IModel model)
    {
        IfcCircleProfileDef profileDef = IfcGeometry.CreateCircleProfileDef(model, Diameter / 2, XbimVector3D.Zero);
        IfcExtrudedAreaSolid extrudedArea = model.Instances.New<IfcExtrudedAreaSolid>(solid =>
        {
            solid.SweptArea = profileDef;
            solid.ExtrudedDirection = IfcAxis.CreateDirection(model, new XbimVector3D(0, 0, 1));
            solid.Depth = Depth;
            solid.Position = IfcAxis.CreateAxis2Placement3D(model, XbimVector3D.Zero, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
            
            _depthChanged += depth => solid.Depth = depth;
            _coordinatesChanged += coordinates => solid.Position = IfcAxis.CreateAxis2Placement3D(model, coordinates, ObjectMatrix3D.Forward, ObjectMatrix3D.Right);
        });
        IfcShapeRepresentation shapeRep = IfcGeometry.CreateShapeRepresentation(model, extrudedArea);
        IfcProductDefinitionShape productDefShape = IfcGeometry.CreateProductDefinitionShape(model, shapeRep);

        return productDefShape;
    }

    private IfcPipeSegment CreatePipe(IModel model, IfcProductDefinitionShape productDefShape)
    {
        IfcLocalPlacement localPlacement = IfcAxis.CreateLocalPlacement(model, ObjectMatrix3D.Translation);
        IfcPipeSegment pipeSegment = model.Instances.New<IfcPipeSegment>(segment =>
        {
            segment.Name = PipeEntity.GetName();
            segment.Tag = "Pipe";
            segment.PredefinedType = IfcPipeSegmentTypeEnum.FLEXIBLESEGMENT;
            segment.ObjectPlacement = localPlacement;
            segment.Representation = productDefShape;
        });

        return pipeSegment;
    }

    private void AddProperties(IModel model)
    {
        #region Pset_PipeSegmentTypeStart

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeSegment);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeSegmentTypeStart";
                foreach (var kvp in PipeEntity.GetData())
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
        
        #region Pset_PipeSegmentTypeCommon

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeSegment);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
            {
                set.Name = "Pset_PipeSegmentTypeCommon";
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "InnerDiameter";
                    value.NominalValue =
                        new IfcPositiveLengthMeasure(PipeEntity.GetOutsideDiameter() - PipeEntity.GetWallThickness());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "OuterDiameter";
                    value.NominalValue = new IfcPositiveLengthMeasure(PipeEntity.GetOutsideDiameter());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                {
                    value.Name = "WorkingPressure";
                    value.NominalValue = new IfcPressureMeasure(PipeEntity.GetPressure());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertyBoundedValue>(value =>
                {
                    value.Name = "PressureRange";
                    value.LowerBoundValue = new IfcPressureMeasure(PipeEntity.GetPressure());
                    value.UpperBoundValue = new IfcPressureMeasure(PipeEntity.GetTestPressure());
                }));
                set.HasProperties.Add(model.Instances.New<IfcPropertyBoundedValue>(value =>
                {
                    value.Name = "TemperatureRange";
                    value.LowerBoundValue = new IfcThermodynamicTemperatureMeasure(PipeEntity.GetTemperature());
                    value.UpperBoundValue = new IfcThermodynamicTemperatureMeasure(PipeEntity.GetTemperature());
                }));
            });
        });

        #endregion

        #region Qto_PipeSegmentBaseQuantities

        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeSegment);
            properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
            {
                quantity.Name = "Qto_PipeSegmentBaseQuantities";
                quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                {
                    length.Name = "Length";
                    length.LengthValue = new IfcLengthMeasure(Depth);
                    
                    _depthChanged += depth => length.LengthValue = new IfcLengthMeasure(depth);
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                {
                    weight.Name = "NetWeight";
                    weight.WeightValue = new IfcMassMeasure(PipeEntity.GetPipeUnitWeight() * Depth);
                    
                    _depthChanged += depth => weight.WeightValue = new IfcMassMeasure(PipeEntity.GetPipeUnitWeight() * depth);
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                {
                    double circumference = Math.PI * PipeEntity.GetOutsideDiameter();
                    area.Name = "OuterSurfaceArea";
                    area.AreaValue = new IfcAreaMeasure(circumference * Depth);

                    _depthChanged += depth => area.AreaValue = new IfcAreaMeasure(circumference * depth);
                }));
            });
        });

        #endregion

        #region DEBUG

        #if DEBUG
        model.Instances.New<IfcRelDefinesByProperties>(properties =>
        {
            properties.RelatedObjects.Add(_pipeSegment);
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