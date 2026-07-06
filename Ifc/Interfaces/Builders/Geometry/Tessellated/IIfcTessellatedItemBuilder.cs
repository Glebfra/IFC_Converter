using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcTessellatedItemBuilder<out T> : IIfcBuilder
        where T : IIfcTessellatedItem
    {
        public T? TessellatedItem { get; }

        public T CreateTessellatedItem(IModel model);
    }
}