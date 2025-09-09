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
    }
}