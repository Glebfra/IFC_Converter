using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Interfaces;
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

        public static Qto_PipeSegmentBaseQuantities CreateFromQuantitySet(IIfcElementQuantity elementQuantity)
        {
            Qto_PipeSegmentBaseQuantities qto = new Qto_PipeSegmentBaseQuantities();
            foreach (IIfcPhysicalQuantity quantity in elementQuantity.Quantities)
            {
                switch (quantity.Name)
                {
                    case nameof(Length):
                        qto.Length = ((IfcQuantityLength)quantity).LengthValue;
                        break;
                    case nameof(GrossCrossSectionArea):
                        qto.GrossCrossSectionArea = ((IfcQuantityArea)quantity).AreaValue;
                        break;
                    case nameof(NetCrossSectionArea):
                        qto.NetCrossSectionArea = ((IfcQuantityArea)quantity).AreaValue;
                        break;
                    case nameof(OuterSurfaceArea):
                        qto.OuterSurfaceArea = ((IfcQuantityArea)quantity).AreaValue;
                        break;
                    case nameof(GrossWeight):
                        qto.GrossWeight = ((IfcQuantityWeight)quantity).WeightValue;
                        break;
                    case nameof(NetWeight):
                        qto.NetWeight = ((IfcQuantityWeight)quantity).WeightValue;
                        break;
                }
            }

            return qto;
        }

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