using Start.Entities.Abstract;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractFittingEntity : IfcAbstractConnectorEntity
    {
        public double Diameter { get; protected set; }

        private StartAbstractEntity _abstractEntity;

        protected IfcAbstractFittingEntity(StartAbstractEntity abstractEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(abstractEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            _abstractEntity = abstractEntity;
            Diameter = AbstractSegmentEntities[0].Diameter;
        }
    }
}