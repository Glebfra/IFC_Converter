using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
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

            return ballExpansionJointEntity;
        }
    }
    
    #endif
}