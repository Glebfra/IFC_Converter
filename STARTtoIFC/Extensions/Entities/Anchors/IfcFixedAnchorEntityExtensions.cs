using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using Start.Entities.Anchors;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Anchors
{
    public static class IfcFixedAnchorEntityExtensions
    {
        public static IfcFixedAnchorEntity CreateFromStart(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFixedAnchorObjectMatrix(nodeEntity, abstractSegmentEntities);
            
            IfcFixedAnchorEntity fixedAnchorEntity = new IfcFixedAnchorEntity(
                anchorEntity.Name,
                anchorEntity.Type.ToString(),
                objectMatrix3D,
                abstractSegmentEntities[0].Diameter * 2,
                abstractSegmentEntities[0].Diameter * 2
            );
            
            fixedAnchorEntity.ConnectedEntities.AddRange(abstractSegmentEntities);
            
            return fixedAnchorEntity;
        }
    }
}