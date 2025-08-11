using IFC.Entities.Abstract.Equipments;
using IFC.Tools;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Equipments.Vertex
{
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
}