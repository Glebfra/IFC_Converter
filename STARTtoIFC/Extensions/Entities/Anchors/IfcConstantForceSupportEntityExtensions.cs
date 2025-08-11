using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal static class IfcConstantForceSupportEntityExtensions
    {
        public static IfcConstantForceSupportEntity CreateFromStart(StartConstantForceSupportEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);

            IfcConstantForceSupportEntity constantForceSupportEntity = new IfcConstantForceSupportEntity(
                constantForceSupport.Name,
                constantForceSupport.Type.ToString(),
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );
            
            constantForceSupportEntity.ConnectedEntities.AddRange(segmentEntities);

            return constantForceSupportEntity;
        }
    }
}