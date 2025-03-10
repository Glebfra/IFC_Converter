using Xbim.Common.Geometry;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Entities.Abstract
{
    public interface IIfcTwoNodeEntity
    {
        public IfcNodeEntity[] NodeEntities { get; }
        public XbimVector3D Direction { get; }
        public IfcDistributionPort[] Ports { get; }
    }
}