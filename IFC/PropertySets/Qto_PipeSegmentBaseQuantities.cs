using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.QuantityResource;

namespace IFC.PropertySets
{
    public class Qto_PipeSegmentBaseQuantities : IPropertySet
    {
        public ActionProperty<IfcLengthMeasure> Length = new ActionProperty<IfcLengthMeasure>(0.0);
        public ActionProperty<IfcAreaMeasure> GrossCrossSectionArea = new ActionProperty<IfcAreaMeasure>(0.0);
        public ActionProperty<IfcAreaMeasure> NetCrossSectionArea = new ActionProperty<IfcAreaMeasure>(0.0);
        public ActionProperty<IfcAreaMeasure> OuterSurfaceArea = new ActionProperty<IfcAreaMeasure>(0.0);
        public ActionProperty<IfcMassMeasure> GrossWeight = new ActionProperty<IfcMassMeasure>(0.0);
        public ActionProperty<IfcMassMeasure> NetWeight = new ActionProperty<IfcMassMeasure>(0.0);

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcElementQuantity>(quantity =>
            {
                quantity.Name = nameof(Qto_PipeSegmentBaseQuantities);
                quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                {
                    length.Name = nameof(Length);
                    length.LengthValue = Length.Value;
                    
                    Length.OnValueChange += () => length.LengthValue = Length.Value;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                {
                    area.Name = nameof(GrossCrossSectionArea);
                    area.AreaValue = GrossCrossSectionArea.Value;
                    
                    GrossCrossSectionArea.OnValueChange += () => area.AreaValue = GrossCrossSectionArea.Value;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                {
                    area.Name = nameof(NetCrossSectionArea);
                    area.AreaValue = NetCrossSectionArea.Value;
                    
                    NetCrossSectionArea.OnValueChange += () => area.AreaValue = NetCrossSectionArea.Value;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(area =>
                {
                    area.Name = nameof(GrossWeight);
                    area.WeightValue = GrossWeight.Value;
                    
                    GrossWeight.OnValueChange += () => area.WeightValue = GrossWeight.Value;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(area =>
                {
                    area.Name = nameof(NetWeight);
                    area.WeightValue = NetWeight.Value;
                    
                    NetWeight.OnValueChange += () => area.WeightValue = NetWeight.Value;
                }));
            });
        }
    }
}