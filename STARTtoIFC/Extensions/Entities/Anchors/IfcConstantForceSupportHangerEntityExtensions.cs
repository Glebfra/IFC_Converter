using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal static class IfcConstantForceSupportHangerEntityExtensions
    {
        public static IfcConstantForceSupportHangerEntity CreateFromStart(StartConstantForceSupportHangerEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);

            IfcConstantForceSupportHangerEntity constantForceSupportHangerEntity = new IfcConstantForceSupportHangerEntity(
                constantForceSupport.Name,
                constantForceSupport.Type.ToString(),
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            constantForceSupportHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            constantForceSupportHangerEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(constantForceSupport));
            
            return constantForceSupportHangerEntity;
        }
    }
}