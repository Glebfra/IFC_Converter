using IFC.Entities.Abstract.Anchors;
using IFC.Tools;
using Start.Entities;
using Start.Entities.Anchors;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Anchors
{
    public class IfcNonStandardRestraintEntity : IfcAbstractNonStandardRestraintEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Diameter { get; }
        public override double Height { get; }
        public override StartNonStandardRestraint NonStandardRestraint { get; }
        public override int NumSegments { get; }
        
        public IfcNonStandardRestraintEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix, StartNonStandardRestraint restraint, double diameter, double height, int numSegments)
            : base(objectMatrix)
        {
            Name = name;
            Tag = tag;
            NonStandardRestraint = restraint;
            Diameter = diameter;
            Height = height;
            NumSegments = numSegments;
        }
    }
}