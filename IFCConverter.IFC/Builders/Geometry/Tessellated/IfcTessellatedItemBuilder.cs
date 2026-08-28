using IFCConverter.IFC.Interfaces.Geometry.Tessellated;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Geometry.Tessellated
{
    public class IfcTessellatedItemBuilder<T> : IIfcTessellatedItemBuilder<T>
        where T : IIfcTessellatedItem, IInstantiableEntity
    {
        public T TessellatedItem { get; private set; }

        public object Instance => TessellatedItem;

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