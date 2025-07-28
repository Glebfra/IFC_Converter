using System.Linq;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Abstract.Fittings
{
    #if NEW

    public abstract class IfcAbstractReducerEntity : IfcAbstractFittingEntity
    {
        protected IfcAbstractReducerEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
    }
    
    #else

    public abstract class IfcAbstractReducerEntity : IfcAbstractFittingEntity
    {
        protected double[] _PipeRadiuses;
        
        protected IfcAbstractReducerEntity(StartReducerEntity reducerEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(reducerEntity, nodeEntity, segmentEntities)
        {
            AbstractSegmentEntities = AbstractSegmentEntities
                .OrderBy(entity => entity.Diameter)
                .ToArray();
            _PipeRadiuses = AbstractSegmentEntities.Select(item => item.Diameter / 2).ToArray();
        }

        protected virtual void ClipPipes()
        {
            AbstractSegmentEntities[1].Clip(NodeEntity, Length);
        }
    }

    #endif
}