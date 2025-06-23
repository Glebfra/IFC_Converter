using Start.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;

namespace IFC.Entities
{
    public class IfcNodeEntity
    {
        public readonly int ID;
        public readonly XbimMatrix3D ObjectMatrix3D;

        public IfcNodeEntity(StartNodeEntity nodeEntity)
        {
            ID = nodeEntity.ID;
            XbimVector3D coordinates = new XbimVector3D(
                nodeEntity.XCoord.SIProperty,
                nodeEntity.YCoord.SIProperty,
                nodeEntity.ZCoord.SIProperty
            );
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, new XbimVector3D(1, 0, 0), new XbimVector3D(0, 0, 1));
        }
    }
}