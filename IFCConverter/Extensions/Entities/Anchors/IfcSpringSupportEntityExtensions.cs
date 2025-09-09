using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
{
    internal static class IfcSpringSupportEntityExtensions
    {
        public static IfcSpringSupportEntity CreateFromStart(StartSpringSupportEntity springSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            IfcSpringSupportEntity springSupportEntity = new IfcSpringSupportEntity(
                springSupport.Name,
                springSupport.Type.ToString(),
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            springSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            springSupportEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(springSupport));

            return springSupportEntity;
        }
    }
}