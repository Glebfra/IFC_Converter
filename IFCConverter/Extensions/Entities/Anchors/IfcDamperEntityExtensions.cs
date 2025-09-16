using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
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
            ifcDamperEntity.PropertySets.Add(Pset_Start.CreateFromStart(damperEntity));

            return ifcDamperEntity;
        }
    }
}