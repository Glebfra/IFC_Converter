using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedComponentElements;

namespace Ifc.Builders.Elements
{
    public class IfcDiscreteAccessoryBuilder<T> : IfcElementBuilder<T>, IIfcDiscreteAccessoryBuilder<T>
        where T : IfcDiscreteAccessory
    {
        public IfcDiscreteAccessoryBuilder(IfcLabel name, IfcIdentifier tag,
            IfcDiscreteAccessoryTypeEnum predefinedType)
            : base(name, tag)
        {
            PredefinedType = predefinedType;
        }

        public IfcDiscreteAccessoryTypeEnum PredefinedType { get; }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.PredefinedType = PredefinedType;
            return instance;
        }
    }
}