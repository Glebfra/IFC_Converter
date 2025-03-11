using IFC.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.SharedBldgServiceElements;

namespace IFC.Entities.Interfaces
{
    public interface IIfcTwoNodeEntity
    {
        public IfcNodeEntity[] NodeEntities { get; }
        public XbimVector3D Direction { get; }
    }
}