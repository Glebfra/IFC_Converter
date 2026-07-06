using Xbim.Common;

namespace Ifc.Interfaces
{
    public interface IIfcBuilder
    {
        public object? Instance { get; }

        public object Build(IModel model);
    }
}