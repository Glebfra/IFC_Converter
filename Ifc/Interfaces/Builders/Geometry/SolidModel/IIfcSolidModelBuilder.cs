using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces
{
    public interface IIfcSolidModelBuilder<out T> : IIfcBuilder
        where T : IIfcSolidModel
    {
        public T? SolidModel { get; }

        public T CreateSolidModel(IModel model);
    }
}