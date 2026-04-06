using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Geometry.SolidModel
{
    public class IfcSolidModelBuilder<T> : IIfcSolidModelBuilder<T>
        where T : IIfcSolidModel, IInstantiableEntity
    {
        public object? Instance => SolidModel;

        public T? SolidModel { get; private set; }

        public virtual T CreateSolidModel(IModel model)
        {
            SolidModel = model.Instances.New<T>();
            return SolidModel;
        }

        public object Build(IModel model)
        {
            return CreateSolidModel(model);
        }
    }
}