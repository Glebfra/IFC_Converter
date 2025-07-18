using IFC.Tools;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class NewIfcAbstractFittingEntity : NewIfcAbstractEntity
    {
        public abstract ActionProperty<double> Length { get; }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            return pipeFitting;
        }
    }
}