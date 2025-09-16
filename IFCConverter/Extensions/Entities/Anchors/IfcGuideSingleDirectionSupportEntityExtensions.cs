using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
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
            guideSingleDirectionSupportEntityIfc.PropertySets.Add(Pset_Start.CreateFromStart(guideSingleDirectionSupportEntity));
            
            return guideSingleDirectionSupportEntityIfc;
        }
    }
}