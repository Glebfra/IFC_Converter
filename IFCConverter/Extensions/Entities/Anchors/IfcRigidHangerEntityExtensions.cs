using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
{
    internal static class IfcRigidHangerEntityExtensions
    {
        public static IfcRigidHangerEntity CreateFromStart(StartRigidHangerEntity rigidHanger, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);
            
            double diameter = segmentEntities[0].Diameter;
            double height = diameter * 2;
            
            string name = rigidHanger.Name;
            string type = rigidHanger.Type.ToString();

            IfcRigidHangerEntity rigidHangerEntity = new IfcRigidHangerEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                diameter,
                height,
                numSegments
            );
            
            rigidHangerEntity.ConnectedEntities.AddRange(segmentEntities);
            rigidHangerEntity.PropertySets.Add(Pset_Start.CreateFromStart(rigidHanger));

            return rigidHangerEntity;
        }
    }
}