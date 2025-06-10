using System.Linq;
using IFC.Entities.Abstract.Fittings;
using IFC.Entities.Abstract.Segments;
using IFC.Tools;
using IFC.Tools.Geometry;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFC.Entities.Fittings.Vertex
{
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
}