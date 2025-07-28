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

    public class IfcVertexLateralExpansionJointEntity : IfcAbstractVertexLateralExpansionJointEntity
    {
        public override ActionProperty<IfcLabel> Name { get; }
        public override ActionProperty<IfcIdentifier> Tag { get; }
        public override ActionProperty<double> Length { get; }
        public override ActionProperty<double> Radius { get; }
        public override ActionProperty<double> Angle { get; }
        public override ActionProperty<int> NumSegments { get; }

        public IfcVertexLateralExpansionJointEntity(IfcLabel name, IfcIdentifier tag, XbimMatrix3D objectMatrix3D, double length, double radius, double angle, int numSegments) 
            : base(objectMatrix3D)
        {
            Name = name;
            Tag = tag;
            Length = length;
            Radius = radius;
            Angle = angle;
            NumSegments = numSegments;
        }
    }
    
    #else
    
    public sealed class IfcVertexLateralExpansionJointEntity : IfcAbstractVertexLateralExpansionJointEntity
    {
        public override double Length { get; protected set; }
        public override int NumSegments { get; protected set; }
        public override double Radius { get; protected set; }
        public override double Angle { get; protected set; }
        
        public IfcVertexLateralExpansionJointEntity(StartLateralExpansionJointEntity lateralExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments) 
            : base(lateralExpansion, nodeEntity, segmentEntities)
        {
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D[] directionToPipes = AbstractSegmentEntities.Select(entity => IfcAxis.GetPipeDirectionFromNode(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();

            Angle = forward.Angle(directionToPipes[1]);
            NumSegments = numSegments;
            Length = lateralExpansion.Length.SIProperty;
            Radius = Length;
        }
    }

    #endif
}