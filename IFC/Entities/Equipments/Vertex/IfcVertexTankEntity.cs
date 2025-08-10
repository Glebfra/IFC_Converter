using IFC.Entities.Abstract.Equipments;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Start.Entities.Equipments;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
    #if NEW

    public class IfcVertexTankEntity : IfcAbstractVertexTankEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> PipeDiameter { get; }
        public override ActionProperty<double> TankHeight { get; }
        public override ActionProperty<double> TankRadius { get; }
        public override ActionProperty<double> FlangeHeight { get; }
        public override ActionProperty<double> FlangeRadius { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcVertexTankEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, 
            double pipeDiameter, double tankHeight, double tankRadius, double flangeHeight, double flangeRadius, int numSegments
            )
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            PipeDiameter = pipeDiameter;
            TankHeight = tankHeight;
            TankRadius = tankRadius;
            FlangeHeight = flangeHeight;
            FlangeRadius = flangeRadius;
            NumSegments = numSegments;
        }
    }
    
    #else
    
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

    #endif
}