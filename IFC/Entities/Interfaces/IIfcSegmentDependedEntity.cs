using IFC.Entities.Abstract.Segments;

namespace IFC.Entities.Interfaces
{
    public interface IIfcSegmentDependedEntity
    {
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
    }
}