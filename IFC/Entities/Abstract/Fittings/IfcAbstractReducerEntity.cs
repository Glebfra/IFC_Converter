using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractReducerEntity : IfcAbstractFittingEntity
    {
        protected IfcAbstractReducerEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(reducerEntity, nodeEntity, segmentEntities)
        {
            
        }
    }
}