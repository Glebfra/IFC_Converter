using IFC.Entities.Abstract;

namespace IFC.Entities.Interfaces
{
    public interface IIfcSegmentDependedEntity
    {
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
    }
}