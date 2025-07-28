using IFC.Entities.Abstract.Anchors;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Anchors.CAD
{
    #if NEW

    public class IfcDamperEntity : IfcAbstractDamperEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Diameter { get; }
        public override StartNonStandardRestraint NonStandardRestraint { get; }
        public override double Height { get; }
        public override int NumSegments { get; }
        
        public IfcDamperEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, double diameter, ) : base(objectMatrix)
        {
            
        }
    }
    
    #else
    
    public sealed class IfcDamperEntity : IfcAbstractDamperEntity
    {
        public override double Diameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Height { get; protected set; }
        
        public IfcDamperEntity(StartDamperEntity damperEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) : base(damperEntity, nodeEntity, segmentEntities)
        {
            NumSegments = 8;
            Diameter = AbstractSegmentEntities[0].Diameter;
            Height = Diameter * 2;
        }
    }

    #endif
}