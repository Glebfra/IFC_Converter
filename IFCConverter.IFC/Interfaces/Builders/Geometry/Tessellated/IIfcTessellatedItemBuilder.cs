using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Geometry.Tessellated
{
    public interface IIfcTessellatedItemBuilder<out T> : IIfcBuilder
        where T : IIfcTessellatedItem
    {
        T TessellatedItem { get; }

        T CreateTessellatedItem(IModel model);
    }
}