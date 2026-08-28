using Xbim.Common;

namespace IFCConverter.IFC.Interfaces
{
    public interface IIfcBuilder
    {
        object Instance { get; }

        object Build(IModel model);
    }
}