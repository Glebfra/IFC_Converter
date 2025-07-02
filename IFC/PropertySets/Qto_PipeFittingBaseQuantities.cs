using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.PropertyResource;
using Xbim.Ifc4.QuantityResource;

namespace IFC.EntitiesExtensions
{
    public class Qto_PipeFittingBaseQuantities : IPropertySet
    {
        public IfcLengthMeasure Length;
        public IfcMassMeasure NetWeight;
        
        public Qto_PipeFittingBaseQuantities() {}

        public static Qto_PipeFittingBaseQuantities CreateFromPropertySet(IIfcPropertySet propertySet)
        {
            Qto_PipeFittingBaseQuantities qto = new Qto_PipeFittingBaseQuantities();
            foreach (IIfcProperty property in propertySet.HasProperties)
            {
                IfcPropertySingleValue singleValue = (IfcPropertySingleValue)property;
                switch (property.Name)
                {
                    case nameof(Length):
                        qto.Length = (IfcLengthMeasure)singleValue.NominalValue;
                        break;
                    case nameof(NetWeight):
                        qto.NetWeight = (IfcMassMeasure)singleValue.NominalValue;
                        break;
                }
            }

            return qto;
        }
        
        public IfcPropertySetDefinitionSelect CreatePropertySet(IModel model)
        {
            return model.Instances.New<IfcElementQuantity>(quantity =>
            {
                quantity.Name = "Qto_PipeFittingBaseQuantities";
                quantity.Quantities.Add(model.Instances.New<IfcQuantityLength>(length =>
                {
                    length.Name = nameof(Length);
                    length.LengthValue = Length;
                }));
                quantity.Quantities.Add(model.Instances.New<IfcQuantityWeight>(weight =>
                {
                    weight.Name = nameof(NetWeight);
                    weight.WeightValue = NetWeight;
                }));
            });
        }
    }
}