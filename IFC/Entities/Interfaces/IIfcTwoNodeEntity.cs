using Xbim.Common.Geometry;

namespace IFC.Entities.Interfaces
{
    #if NEW
    
    public interface IIfcTwoNodeEntity
    {
        public IfcNodeEntity[] NodeEntities { get; }
    }
    
    #else
    
    public interface IIfcTwoNodeEntity
    {
        public IfcNodeEntity[] NodeEntities { get; }
        public XbimVector3D Direction { get; }
    }
    
    #endif
}