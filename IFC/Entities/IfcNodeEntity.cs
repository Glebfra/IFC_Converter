using Start.Entities;
using Xbim.Common.Geometry;

namespace IFC.Entities
{
    public class IfcNodeEntity
    {
        public XbimMatrix3D ObjectMatrix3D { get; protected set; }

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