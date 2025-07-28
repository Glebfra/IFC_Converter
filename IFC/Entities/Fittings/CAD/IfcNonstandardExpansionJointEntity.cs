using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW

    public class IfcNonstandardExpansionJointEntity : IfcAbstractNonStandardExpansionJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double Radius { get; }

        public IfcNonstandardExpansionJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double radius) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Radius = radius;
        }
    }
    
    #else
    
    public sealed class IfcNonstandardExpansionJointEntity : IfcAbstractNonStandardExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double Radius { get; protected set; }
        
        public IfcNonstandardExpansionJointEntity(StartNonstandardExpansionJointEntity nonstandardExpansion, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(nonstandardExpansion, ifcNodeEntity, segmentEntities)
        {
            Length = nonstandardExpansion.Length.SIProperty;
            Radius = segmentEntities[0].Diameter / 2;
        }
    }

    #endif
}