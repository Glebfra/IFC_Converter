using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using Xbim.Common.Geometry;

namespace IFC.Extensions
{
    public static class IfcAbstractSegmentEntityExtensions
    {
        public static IndexedResult<IfcNodeEntity> GetNearestNode(this IfcAbstractSegmentEntity segmentEntity, IfcNodeEntity nodeEntity)
        {
            return segmentEntity.NodeEntities
                .Select((item, index) => new IndexedResult<IfcNodeEntity>() {Object = item, Index = index})
                .OrderBy(node => node.Object.GetDistanceToAnotherNode(nodeEntity))
                .First();
        }

        public static IfcAbstractSegmentEntity[] GetNearestSegments(this IEnumerable<IfcAbstractSegmentEntity> segmentEntities, IfcNodeEntity nodeEntity, int count)
        {
            return segmentEntities
                .OrderBy(entity => GetNearestNode(entity, nodeEntity).Object.GetDistanceToAnotherNode(nodeEntity))
                .Take(count)
                .ToArray();
        }

        public static IfcAbstractSegmentEntity[] GetConnSegments(this IEnumerable<IfcAbstractSegmentEntity> segmentEntities, IfcNodeEntity nodeEntity)
        {
            return segmentEntities
                .Where(item => item.NodeEntities.Contains(nodeEntity))
                .ToArray();
        }

        public static XbimVector3D ReplaceNearestNode(this IfcAbstractSegmentEntity segmentEntity, IfcNodeEntity nodeEntity)
        {
            IndexedResult<IfcNodeEntity> nearestNode = GetNearestNode(segmentEntity, nodeEntity);
            XbimVector3D displacement = nearestNode.Object.GetDisplacementToAnotherNode(nodeEntity);
            segmentEntity.NodeEntities[nearestNode.Index] = nodeEntity;
            return displacement;
        }
    }
}