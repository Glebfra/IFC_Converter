using IFC.Entities;
using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using Start.API;
using Start.Entities.Abstract;
using Start.Entities.Segments;
using STARTtoIFC.Extensions.Entities;
using STARTtoIFC.Extensions.Entities.Segments;

namespace STARTtoIFC.Extensions
{
    #if NEW
    
    internal static class IfcEntityFactory
    {
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.PIPE_ELEMENT:
                    return IfcPipeSegmentEntityExtensions.CreateFromStart((StartPipeEntity)entity, nodeEntities);
                
                case StartElementType.CYLINDRICAL_SHELL:
                    return IfcCylindricalShellEntityExtensions.CreateFromStart((StartPipeEntity)entity, nodeEntities);

                default:
                    return null;
            }
        }

        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                case StartElementType.RIGID_ELEMENT:
                    return IfcRigidElementEntityExtensions.CreateFromStart((StartRigidElementEntity)entity, nodeEntities, segmentEntities);
                
                case StartElementType.FLEXIBLE_ELEMENT:
                    return IfcFlexibleSegmentEntityExtensions.CreateFromStart((StartFlexibleElementEntity)entity, nodeEntities, segmentEntities);

                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            switch (entity.Type)
            {
                default:
                    return null;
            }
        }
        
        public static IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            switch (entity.Type)
            {
                default:
                    return null;
            }
        }
    }
    
    #endif
}