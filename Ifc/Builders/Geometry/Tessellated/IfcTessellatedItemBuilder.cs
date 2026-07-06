using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.Tessellated
{
    public class IfcTessellatedItemBuilder<T> : IIfcTessellatedItemBuilder<T>
        where T : IIfcTessellatedItem, IInstantiableEntity
    {
        public T? TessellatedItem { get; private set; }

        public object? Instance => TessellatedItem;

        public virtual T CreateTessellatedItem(IModel model)
        {
            TessellatedItem = model.Instances.New<T>();
            return TessellatedItem;
        }

        public object Build(IModel model)
        {
            return CreateTessellatedItem(model);
        }
    }
}