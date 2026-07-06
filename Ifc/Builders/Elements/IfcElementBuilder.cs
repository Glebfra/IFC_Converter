using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;

namespace Ifc.Builders.Elements
{
    public class IfcElementBuilder<T> : IfcProductBuilder<T>, IIfcElementBuilder<T>
        where T : IfcElement, IInstantiableEntity
    {
        public IfcElementBuilder(IfcLabel name, IfcIdentifier tag)
        {
            Name = name;
            Tag = tag;
        }

        public IfcLabel Name { get; }
        public IfcIdentifier Tag { get; }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.Name = Name;
            instance.Tag = Tag;
            return instance;
        }
    }
}