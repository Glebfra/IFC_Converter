using IFC.Entities.Interfaces;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractFittingEntity : IfcAbstractEntity, IIfcOneNodeEntity
    {
        public abstract ActionProperty<double> Length { get; }
        public IfcNodeEntity NodeEntity { get; }

        protected IfcAbstractFittingEntity(XbimMatrix3D objectMatrix3D)
            : base(objectMatrix3D)
        {
            NodeEntity = new IfcNodeEntity(objectMatrix3D);
        }

        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            return pipeFitting;
        }
    }
}