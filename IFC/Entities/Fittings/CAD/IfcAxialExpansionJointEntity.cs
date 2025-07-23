using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.CAD
{
    #if NEW
    
    public class IfcAxialExpansionJointEntity : IfcAbstractAxialExpansionJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override double Diameter { get; }
        public override int NumSegments { get; }

        public IfcAxialExpansionJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter, int numSegments)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Length = new ActionProperty<double>(length);
            Diameter = diameter;
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcAxialExpansionJointEntity : IfcAbstractAxialExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double PipeDiameter { get; protected set; }

        public IfcAxialExpansionJointEntity(StartAxialExpansionJointEntity axialExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(axialExpansionJoint, nodeEntity, segmentEntities)
        {
            Length = axialExpansionJoint.Length.SIProperty;
            PipeDiameter = segmentEntities[0].Diameter;
        }
    }

    #endif
}