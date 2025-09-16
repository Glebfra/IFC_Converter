using IFC.Tools;
using Start.Entities.Abstract;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.QuantityResource;

namespace IFC.PropertySets
{
    public class Qto_PipeFittingBaseQuantities : IPropertySet
    {
        public ActionProperty<IfcLengthMeasure> Length = new ActionProperty<IfcLengthMeasure>(0.0);
        public ActionProperty<IfcMassMeasure> NetWeight = new ActionProperty<IfcMassMeasure>(0.0);

        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcElementQuantity>(quantity =>
            {
                quantity.Name = "Qto_PipeFittingBaseQuantities";
                quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                {
                    length.Name = nameof(Length);
                    length.LengthValue = Length.Value;

                    Length.OnValueChange += () => length.LengthValue = Length.Value;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                {
                    weight.Name = nameof(NetWeight);
                    weight.WeightValue = NetWeight.Value;
                    
                    NetWeight.OnValueChange += () => weight.WeightValue = NetWeight.Value;
                }));
            });
        }
        
        public static Qto_PipeFittingBaseQuantities CreateFromStart(StartAbstractFittingEntity fittingEntity)
        {
            Qto_PipeFittingBaseQuantities qto = new Qto_PipeFittingBaseQuantities()
            {
                NetWeight = new ActionProperty<IfcMassMeasure>(fittingEntity.Weight.SIProperty),
            };

            return qto;
        }
        
        public static Qto_PipeFittingBaseQuantities CreateFromPropertySet(IIfcElementQuantity propertySet)
        {
            Qto_PipeFittingBaseQuantities qto = new Qto_PipeFittingBaseQuantities();
            foreach (IIfcPhysicalQuantity quantity in propertySet.Quantities)
            {
                switch (quantity.Name)
                {
                    case nameof(qto.Length):
                        qto.Length = ((IfcQuantityLength)quantity).LengthValue;
                        break;
                    case nameof(qto.NetWeight):
                        qto.NetWeight = ((IfcQuantityWeight)quantity).WeightValue;
                        break;
                }
            }

            return qto;
        }
    }
}