using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal class IfcSlidingSupportEntityExtensions
    {
        public static IfcSlidingSupportEntity CreateFromStart(StartSlidingSupportEntity slidingSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;

            IfcSlidingSupportEntity slidingSupportEntity = new IfcSlidingSupportEntity(
                slidingSupport.Name,
                slidingSupport.Type.ToString(),
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            slidingSupportEntity.ConnectedEntities.AddRange(segmentEntities);

            return slidingSupportEntity;
        }
    }
}