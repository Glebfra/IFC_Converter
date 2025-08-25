using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal static class IfcDamperEntityExtensions
    {
        public static IfcDamperEntity CreateFromStart(StartDamperEntity damperEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            IfcDamperEntity ifcDamperEntity = new IfcDamperEntity(
                damperEntity.Name,
                damperEntity.Type.ToString(),
                objectMatrix3D,
                damperEntity,
                diameter,
                height,
                numSegments
            );
            
            ifcDamperEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcDamperEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(damperEntity));

            return ifcDamperEntity;
        }
    }
}