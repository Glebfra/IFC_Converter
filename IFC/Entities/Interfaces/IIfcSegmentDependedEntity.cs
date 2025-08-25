using System.Collections.Generic;
using IFC.Entities.Abstract.Segments;

namespace IFC.Entities.Interfaces
{
    public interface IIfcSegmentDependedEntity
    {
        public IEnumerable<IfcAbstractSegmentEntity> AbstractSegmentEntities { get; }
    }
}