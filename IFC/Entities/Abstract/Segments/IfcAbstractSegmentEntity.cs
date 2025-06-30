using System.Linq;
using IFC.Entities.Interfaces;
using IFC.Tools;
using Start.Entities.Abstract;
using Start.StartProperties;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;

namespace IFC.Entities.Abstract.Segments
{
    internal struct Node
    {
        public IfcNodeEntity NodeEntity;
        public int Index;
    }
    
    public abstract class IfcAbstractSegmentEntity : IfcAbstractEntity, IIfcTwoNodeEntity, IIfcClippable
    {
        public virtual double Length { get; protected set; }
        public virtual double Diameter { get; protected set; }
        public virtual ActionProperty<double> RealLength { get; protected set; }
        public virtual ActionProperty<double> OuterSurfaceArea { get; protected set; }
        public virtual ActionProperty<XbimVector3D> Coordinates { get; protected set; }
        
        public virtual XbimVector3D Direction { get; }
        public IfcNodeEntity[] NodeEntities { get; }

        private StartAbstractSegmentEntity _segmentEntity;

        protected IfcAbstractSegmentEntity(StartAbstractSegmentEntity segmentEntity, IfcNodeEntity[] nodeEntities) 
            : base(segmentEntity)
        {
            _segmentEntity = segmentEntity;
            NodeEntities = nodeEntities;
        }

        internal Node GetNearestNode(IfcNodeEntity nodeEntity)
        {
            return NodeEntities
                .Select((item, index) => new Node() { NodeEntity = item, Index = index })
                .OrderBy(node => (node.NodeEntity.ObjectMatrix3D.Translation - nodeEntity.ObjectMatrix3D.Translation).Modulus)
                .Single();
        }

        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            if (IsStartNode(nodeEntity))
                Coordinates.Value += ObjectMatrix3D.Forward * clipLength;
            RealLength.Value -= clipLength;
        }

        protected override void AddProperties(IModel model, IfcProduct product)
        {
            base.AddProperties(model, product);

            #region Pset_PipeSegmentTypeCommon

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcPropertySet>(set =>
                {
                    set.Name = "Pset_PipeSegmentTypeCommon";
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "OuterDiameter";
                        value.NominalValue = new IfcPositiveLengthMeasure(Diameter);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "InnerDiameter";
                        value.NominalValue = new IfcPositiveLengthMeasure(_segmentEntity.InnerDiameter.SIProperty);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "NominalDiameter";
                        value.NominalValue = new IfcPositiveLengthMeasure(_segmentEntity.Diameter.SIProperty);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertySingleValue>(value =>
                    {
                        value.Name = "WorkingPressure";
                        value.NominalValue = new IfcPressureMeasure(_segmentEntity.Pressure.SIProperty);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertyBoundedValue>(value =>
                    {
                        PressureProperty[] pressureRange = _segmentEntity.PressureRange;
                        
                        value.Name = "PressureRange";
                        value.LowerBoundValue = new IfcPressureMeasure(pressureRange[0].SIProperty);
                        value.UpperBoundValue = new IfcPressureMeasure(pressureRange[1].SIProperty);
                    }));
                    set.HasProperties.Add(model.Instances.New<IfcPropertyBoundedValue>(value =>
                    {
                        TemperatureProperty[] temperatureRange = _segmentEntity.TemperatureRange;
                        
                        value.Name = "TemperatureRange";
                        value.LowerBoundValue = new IfcThermodynamicTemperatureMeasure(temperatureRange[0].SIProperty);
                        value.UpperBoundValue = new IfcThermodynamicTemperatureMeasure(temperatureRange[1].SIProperty);
                    }));
                });
            });

            #endregion

            #region Qto_PipeSegmentBaseQuantities

            model.Instances.New<IfcRelDefinesByProperties>(properties =>
            {
                properties.RelatedObjects.Add(product);
                properties.RelatingPropertyDefinition = model.Instances.New<IfcElementQuantity>(quantity =>
                {
                    quantity.Name = "Qto_PipeSegmentBaseQuantities";
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                    {
                        length.Name = "Length";
                        length.LengthValue = new IfcLengthMeasure(RealLength.Value);

                        RealLength.OnValueChange += () => length.LengthValue = new IfcLengthMeasure(RealLength.Value);
                    }));
                    quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                    {
                        area.Name = "OuterSurfaceArea";
                        area.AreaValue = new IfcAreaMeasure(OuterSurfaceArea.Value);

                        OuterSurfaceArea.OnValueChange += () => area.AreaValue = new IfcAreaMeasure(OuterSurfaceArea.Value);
                    }));
                });
            });

            #endregion
        }
        
        private bool IsStartNode(IfcNodeEntity nodeEntity)
        {
            XbimVector3D nodeCoordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D startPipeCoordinates = ObjectMatrix3D.Translation;
            XbimVector3D endPipeCoordinates = ObjectMatrix3D.Translation + ObjectMatrix3D.Forward * RealLength.Value;

            return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
        }
    }
}