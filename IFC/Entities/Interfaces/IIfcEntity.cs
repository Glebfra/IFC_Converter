using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Interfaces
{
    public interface IIfcEntity
    {
        public IfcProduct CreateAndAdd(IModel model);
    }
}