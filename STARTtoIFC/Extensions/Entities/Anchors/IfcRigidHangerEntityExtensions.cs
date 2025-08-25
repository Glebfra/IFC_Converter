using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    internal static class IfcRigidHangerEntityExtensions
    {
        public static IfcRigidHangerEntity CreateFromStart(StartRigidHangerEntity rigidHanger, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;

            IfcRigidHangerEntity rigidHangerEntity = new IfcRigidHangerEntity(
                rigidHanger.Name,
                rigidHanger.Type.ToString(),
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            rigidHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            rigidHangerEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(rigidHanger));

            return rigidHangerEntity;
        }
    }
}