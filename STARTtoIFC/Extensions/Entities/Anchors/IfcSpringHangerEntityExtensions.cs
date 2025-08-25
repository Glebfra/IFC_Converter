using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal class IfcSpringHangerEntityExtensions
    {
        public static IfcSpringHangerEntity CreateFromStart(StartSpringSupportEntity springSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;

            IfcSpringHangerEntity springHangerEntity = new IfcSpringHangerEntity(
                springSupport.Name,
                springSupport.Type.ToString(),
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            springHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            springHangerEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(springSupport));

            return springHangerEntity;
        }
    }
}