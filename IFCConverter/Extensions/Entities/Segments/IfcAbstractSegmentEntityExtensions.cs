using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Segments
{
    internal static class IfcAbstractSegmentEntityExtensions
    {
        public static IEnumerable<IfcAbstractSegmentEntity> GetNearestSegments(this IEnumerable<IfcAbstractSegmentEntity> abstractSegmentEntities, XbimVector3D point, int count)
        {
            return abstractSegmentEntities
                .Select(segment => new { Segment = segment, NearestNode = segment.NodeEntities.GetNearestNode(point) })
                .OrderBy(entity => entity.NearestNode.Object.GetDistanceToPoint(point))
                .Take(count)
                .Select(item => item.Segment);
        }

        public static bool IsContainPoint(this IfcAbstractSegmentEntity abstractSegmentEntity, XbimVector3D point, double precision = 1e-3)
        {
            return abstractSegmentEntity.NodeEntities.Any(nodeEntity => nodeEntity.GetDistanceToPoint(point) < precision);
        }

        public static IEnumerable<IfcAbstractSegmentEntity> GetConnectedSegments(this IEnumerable<IfcAbstractSegmentEntity> abstractSegmentEntities, IReadOnlyList<XbimVector3D> boundPoints, double precision = 1e-3)
        {
            return abstractSegmentEntities
                .Where(segment => boundPoints.Any(boundPoint => segment.IsContainPoint(boundPoint, precision)))
                .Take(boundPoints.Count);
        }
    }
}