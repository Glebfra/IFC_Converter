using Start.Entities.Abstract;

namespace IFC.Entities.Abstract
{
    public abstract class IfcAbstractEquipmentEntity : IfcAbstractConnectorEntity
    {
        protected IfcAbstractEquipmentEntity(StartAbstractEntity abstractEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(abstractEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            
        }
    }
}