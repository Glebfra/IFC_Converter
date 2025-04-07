using Xbim.Common.Geometry;

namespace IFC.Entities.Interfaces
{
    public interface IIfcTwoNodeEntity : IIfcEntity
    {
        public IfcNodeEntity[] NodeEntities { get; }
        public XbimVector3D Direction { get; }
    }
}