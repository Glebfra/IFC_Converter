using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW
    
    public class IfcCapEntity : IfcAbstractCapEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double Diameter { get; }

        public IfcCapEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter)
            : base(objectMatrix3D)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            Length = new ActionProperty<double>(length);
            Diameter = diameter;
        }
    }
    
    #else
    
    public sealed class IfcCapEntity : IfcAbstractCapEntity
    {
        public override double Length { get; protected set; }
        public override double Diameter { get; protected set; }
        
        public IfcCapEntity(StartCapEntity capEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(capEntity, nodeEntity, segmentEntities)
        {
            Diameter = segmentEntities[0].Diameter;
            Length = Diameter / 2;
        }
    }
    
    #endif
}