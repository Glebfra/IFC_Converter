using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal static class IfcGuideSingleDirectionSupportEntityExtensions
    {
        public static IfcGuideSingleDirectionSupportEntity CreateFromStart(StartGuideSingleDirectionSupportEntity guideSingleDirectionSupportEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            IfcGuideSingleDirectionSupportEntity guideSingleDirectionSupportEntityIfc = new IfcGuideSingleDirectionSupportEntity(
                guideSingleDirectionSupportEntity.Name,
                guideSingleDirectionSupportEntity.Type.ToString(),
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            guideSingleDirectionSupportEntityIfc.ConnectedEntities.AddRange(segmentEntities);
            
            return guideSingleDirectionSupportEntityIfc;
        }
    }
}