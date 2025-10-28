using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
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
            slidingSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(slidingSupport));

            return slidingSupportEntity;
        }
    }
}