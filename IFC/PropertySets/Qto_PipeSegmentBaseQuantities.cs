using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.QuantityResource;

namespace IFC.PropertySets
{
    public class Qto_PipeSegmentBaseQuantities : IPropertySet
    {
        public IfcLengthMeasure Length;
        public IfcAreaMeasure GrossCrossSectionArea;
        public IfcAreaMeasure NetCrossSectionArea;
        public IfcAreaMeasure OuterSurfaceArea;
        public IfcMassMeasure GrossWeight;
        public IfcMassMeasure NetWeight;

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcElementQuantity>(quantity =>
            {
                quantity.Name = nameof(Qto_PipeSegmentBaseQuantities);
                quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                {
                    length.Name = nameof(Length);
                    length.LengthValue = Length;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                {
                    area.Name = nameof(GrossCrossSectionArea);
                    area.AreaValue = GrossCrossSectionArea;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityArea>(area =>
                {
                    area.Name = nameof(NetCrossSectionArea);
                    area.AreaValue = NetCrossSectionArea;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(area =>
                {
                    area.Name = nameof(GrossWeight);
                    area.WeightValue = GrossWeight;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(area =>
                {
                    area.Name = nameof(NetWeight);
                    area.WeightValue = NetWeight;
                }));
            });
        }
    }
}