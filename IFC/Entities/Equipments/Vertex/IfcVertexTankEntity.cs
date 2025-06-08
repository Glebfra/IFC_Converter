using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using Start.Entities.Equipments;
using Xbim.Common.Geometry;

namespace IFC.Entities.Equipments.Vertex
{
    public sealed class IfcVertexTankEntity : IfcAbstractVertexTankEntity
    {
        public override int NumSegments { get; protected set; }
        public override double PipeDiameter { get; protected set; }
        public override double TankHeight { get; protected set; }
        public override double TankRadius { get; protected set; }
        public override double FlangeHeight { get; protected set; }
        public override double FlangeRadius { get; protected set; }
        
        public IfcVertexTankEntity(StartTankEntity startTankEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
            : base(startTankEntity, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;

            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = VectorExtensions.Z;
            XbimVector3D up = VectorExtensions.Y;
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);

            PipeDiameter = segmentEntities[0].Diameter;
            TankHeight = startTankEntity.DistanceToNozzleAxis.SIProperty * 2;
            TankRadius = startTankEntity.Radius.SIProperty;
            FlangeHeight = PipeDiameter * 0.2;
            FlangeRadius = PipeDiameter * 0.75;
        }
    }
}