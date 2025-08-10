using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    #if NEW
    
    internal static class IfcGuideDoubleDirectionSupportEntityExtensions
    {
        public static IfcGuideDoubleDirectionSupportEntity CreateFromStart(StartGuideDoubleDirectionSupportEntity doubleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            IfcGuideDoubleDirectionSupportEntity guideDoubleDirectionSupportEntity = new IfcGuideDoubleDirectionSupportEntity(
                doubleDirectionSupportEntity.Name,
                doubleDirectionSupportEntity.Type.ToString(),
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            guideDoubleDirectionSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            
            return guideDoubleDirectionSupportEntity;
        }
    }

    #endif
}