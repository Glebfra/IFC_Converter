using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.Vertex
{
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
            XbimVector3D[] directionToPipes = AbstractSegmentEntities.Select(entity => IfcAxis.GetDirectionToPipe(entity, coordinates)).ToArray();
            XbimVector3D forward = directionToPipes[0].Negated();
            
            Angle = forward.Angle(directionToPipes[1]);
            NumSegments = numSegments;
            Length = angularExpansion.Length.SIProperty;
            Radius = Length / 2;
        }
    }
}