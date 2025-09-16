using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
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
            springHangerEntity.PropertySets.Add(Pset_Start.CreateFromStart(springSupport));

            return springHangerEntity;
        }
    }
}