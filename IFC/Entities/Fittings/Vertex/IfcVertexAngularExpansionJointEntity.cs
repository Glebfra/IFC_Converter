using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities.Fittings.Vertex
{
    #if NEW
    
    public class IfcVertexAngularExpansionJointEntity : IfcAbstractVertexAngularExpansionJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<XbimMatrix3D> ObjectMatrix3D { get; }
        public override ActionProperty<double> Length { get; }
        public override double Angle { get; }
        public override double Diameter { get; }
        public override int NumSegments { get; }

        public IfcVertexAngularExpansionJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double angle, double diameter, int numSegments)
        {
            Name = new ActionProperty<IfcLabel>(name);
            Tag = new ActionProperty<IfcIdentifier>(tag);
            ObjectMatrix3D = new ActionProperty<XbimMatrix3D>(objectMatrix3D);
            Length = new ActionProperty<double>(length);
            Angle = angle;
            Diameter = diameter;
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexAngularExpansionJointEntity : IfcAbstractVertexAngularExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override double Radius { get; protected set; }
        public override double Angle { get; protected set; }
        public override int NumSegments { get; protected set; }

        public IfcVertexAngularExpansionJointEntity(StartAngularExpansionJointEntity angularExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(angularExpansion, nodeEntity, segmentEntities)
        {
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = AbstractSegmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            
            Angle = forward.Angle(directionToPipes[1]);
            NumSegments = numSegments;
            Length = angularExpansion.Length.SIProperty;
            Radius = Length / 2;
        }
    }

    #endif
}