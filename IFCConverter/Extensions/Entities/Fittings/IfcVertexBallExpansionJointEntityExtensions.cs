using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexBallExpansionJointEntityExtensions
    {
        public static IfcVertexBallExpansionJointEntity CreateFromStart(StartBallExpansionJointEntity ballExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = ballExpansionJoint.Length.SIProperty;
            double diameter = length * 2;

            IfcVertexBallExpansionJointEntity ballExpansionJointEntity = new IfcVertexBallExpansionJointEntity(
                ballExpansionJoint.Name,
                ballExpansionJoint.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            ballExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            ballExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(ballExpansionJoint));
            ballExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(ballExpansionJoint));

            return ballExpansionJointEntity;
        }
    }
}