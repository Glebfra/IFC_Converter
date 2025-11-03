using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Anchors;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Anchors
{
    internal static class IfcFixedAnchorEntityExtensions
    {
        public static IfcFixedAnchorEntity CreateFromStart(StartAnchorEntity anchorEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFixedAnchorObjectMatrix(nodeEntity, abstractSegmentEntities);
            
            string name = anchorEntity.Name;
            string type = anchorEntity.Type.ToString();
            
            IfcFixedAnchorEntity fixedAnchorEntity = new IfcFixedAnchorEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                abstractSegmentEntities[0].Diameter * 2,
                abstractSegmentEntities[0].Diameter * 2
            );
            
            fixedAnchorEntity.ConnectedEntities.AddRange(abstractSegmentEntities);
            fixedAnchorEntity.PropertySets.Add(Pset_Start.CreateFromStart(anchorEntity));
            
            return fixedAnchorEntity;
        }
    }
}