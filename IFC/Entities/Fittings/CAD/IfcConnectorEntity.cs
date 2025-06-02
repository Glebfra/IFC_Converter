using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcConnectorEntity : IfcAbstractConnectorEntity
    {
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }

        public IfcConnectorEntity(StartConnectorEntity connectorEntity, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(connectorEntity, ifcNodeEntity, abstractSegmentEntities)
        {
            Diameter = abstractSegmentEntities[0].Diameter;
            Length = Diameter / 4;
        }
    }
}