using Start.API;
using Start.Entities;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFC.Entities
{
    public class IfcNodeEntity
    {
        public readonly StartNodeEntity NodeEntity;
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
            NodeEntity = nodeEntity;
        }

        public static IfcNodeEntity CreateFromIfc(XbimVector3D coordinates, int id)
        {
            StartNodeEntity nodeEntity = new StartNodeEntity()
            {
                ID = id,
                XCoord = new LengthProperty(coordinates.X),
                YCoord = new LengthProperty(coordinates.Y),
                ZCoord = new LengthProperty(coordinates.Z),
                Type = StartElementType.NODE,
            };

            return new IfcNodeEntity(nodeEntity);
        }
    }
}