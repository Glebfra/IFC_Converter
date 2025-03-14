using Start.Entities;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings
{
    public class IfcNodeEntity
    {
        public readonly XbimMatrix3D ObjectMatrix3D;

        public IfcNodeEntity(StartNodeEntity nodeEntity)
        {
            XbimVector3D coordinates = new XbimVector3D(
                nodeEntity.XCoord,
                nodeEntity.YCoord,
                nodeEntity.ZCoord
            );
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
        }
    }
}