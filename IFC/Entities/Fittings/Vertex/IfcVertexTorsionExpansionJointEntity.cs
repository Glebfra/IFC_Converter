using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW

    public class IfcVertexTorsionExpansionJointEntity : IfcAbstractVertexTorsionExpansionJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Diameter { get; }
        public override ActionProperty<int> NumSegments { get; }
        
        public IfcVertexTorsionExpansionJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter, int numSegments)
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Diameter = diameter;
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexTorsionExpansionJointEntity : IfcAbstractVertexTorsionExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Radius { get; protected set; }

        public IfcVertexTorsionExpansionJointEntity(StartTorsionExpansionJointEntity torsionExpansionJoint, IfcNodeEntity ifcNodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities, int numSegments) 
            : base(torsionExpansionJoint, ifcNodeEntity, abstractSegmentEntities)
        {
            NumSegments = numSegments;
            Radius = abstractSegmentEntities[0].Diameter / 2;
            Length = torsionExpansionJoint.Length.SIProperty;
        }
    }

    #endif
}