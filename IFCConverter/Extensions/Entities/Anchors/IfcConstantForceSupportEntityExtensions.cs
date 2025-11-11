using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Abstract;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
{
    internal static class IfcConstantForceSupportEntityExtensions
    {
        public static IfcConstantForceSupportEntity CreateFromStart(StartConstantForceSupportEntity constantForceSupport, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateStandardObjectMatrix(nodeEntity);

            string name = constantForceSupport.Name;
            string type = constantForceSupport.Type.ToString();

            IfcConstantForceSupportEntity constantForceSupportEntity = new IfcConstantForceSupportEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                segmentEntities[0].Diameter,
                segmentEntities[0].Diameter * 2,
                numSegments
            );

            constantForceSupportEntity.ConnectedEntities.AddRange(segmentEntities);
            constantForceSupportEntity.PropertySets.Add(Pset_Start.CreateFromStart(constantForceSupport));

            return constantForceSupportEntity;
        }
    }
}