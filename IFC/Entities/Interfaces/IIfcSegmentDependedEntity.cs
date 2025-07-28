using System.Collections.Generic;
using IFC.Entities.Abstract.Segments;

namespace IFC.Entities.Interfaces
{
    #if NEW
    
    public interface IIfcSegmentDependedEntity
    {
        public IEnumerable<IfcAbstractSegmentEntity> AbstractSegmentEntities { get; }
    }
    
    #else
    
    public interface IIfcSegmentDependedEntity
    {
        public IfcAbstractSegmentEntity[] AbstractSegmentEntities { get; set; }
    }

    #endif
}