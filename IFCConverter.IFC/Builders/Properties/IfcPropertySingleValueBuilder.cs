using IFCConverter.IFC.Interfaces.Properties;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Builders.Properties
{
    public class IfcPropertySingleValueBuilder<T> : IfcSimplePropertyBuilder<T>, IIfcPropertySingleValueBuilder<T>
        where T : IIfcPropertySingleValue, IInstantiableEntity
    {
        public IfcPropertySingleValueBuilder(IfcIdentifier name, IfcText description, IIfcValue nominalValue,
            IIfcUnit unit)
            : base(name, description)
        {
            NominalValue = nominalValue;
            Unit = unit;
        }

        public IIfcValue NominalValue { get; }
        public IIfcUnit Unit { get; }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.NominalValue = NominalValue;
            instance.Unit = Unit;
            return instance;
        }
    }
}