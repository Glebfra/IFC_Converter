using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFCConverter.Tools;
using Start.API;
using Start.Entities;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities
{
    internal static class IfcNodeEntityExtensions
    {
        private static int _id;
        
        public static StartNodeEntity ToStartEntity(this IfcNodeEntity ifcNodeEntity)
        {
            StartNodeEntity startNodeEntity = new StartNodeEntity();
            startNodeEntity.Name = _id++.ToString();
            startNodeEntity.Type = StartElementType.NODE;

            XbimVector3D coordinates = ifcNodeEntity.ObjectMatrix3D.Translation;
            startNodeEntity.XCoord = LengthProperty.CreateFromSi(coordinates.X);
            startNodeEntity.YCoord = LengthProperty.CreateFromSi(coordinates.Y);
            startNodeEntity.ZCoord = LengthProperty.CreateFromSi(coordinates.Z);

            return startNodeEntity;
        }

        public static IndexedResult<IfcNodeEntity> GetNearestNode(this IEnumerable<IfcNodeEntity> nodeEntities, IfcNodeEntity other)
        {
            return nodeEntities
                .Select((node, index) => new IndexedResult<IfcNodeEntity>(node, index))
                .OrderBy(result => result.Object.GetDistanceToNode(other))
                .First();
        }

        public static IndexedResult<IfcNodeEntity> GetNearestNode(this IEnumerable<IfcNodeEntity> nodeEntities, XbimVector3D point)
        {
            return nodeEntities
                .Select((node, index) => new IndexedResult<IfcNodeEntity>(node, index))
                .OrderBy(result => result.Object.GetDistanceToPoint(point))
                .First();
        }
    }
}