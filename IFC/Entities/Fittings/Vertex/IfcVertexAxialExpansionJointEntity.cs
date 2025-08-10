using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW

    public class IfcVertexAxialExpansionJointEntity : IfcAbstractVertexAxialExpansionJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override double Diameter { get; }
        public override int NumSegments { get; }
        
        public IfcVertexAxialExpansionJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double diameter, int numSegments)
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
    
    public sealed class IfcVertexAxialExpansionJointEntity : IfcAbstractVertexAxialExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double PipeDiameter { get; protected set; }
        public override int NumSegments { get; protected set; }
        
        public IfcVertexAxialExpansionJointEntity(StartAxialExpansionJointEntity expansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(expansionJoint, nodeEntity, segmentEntities)
        {
            NumSegments = numSegments;
            Length = expansionJoint.Length.SIProperty;
            PipeDiameter = segmentEntities[0].Diameter;
        }
    }

    #endif
}