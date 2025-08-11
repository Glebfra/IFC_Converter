using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    internal static class IfcVertexTorsionExpansionJointEntityExtensions
    {
        public static IfcVertexTorsionExpansionJointEntity CreateFromStart(StartTorsionExpansionJointEntity torsionExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = torsionExpansionJoint.Length.SIProperty;

            IfcVertexTorsionExpansionJointEntity vertexTorsionExpansionJointEntity = new IfcVertexTorsionExpansionJointEntity(
                torsionExpansionJoint.Name,
                torsionExpansionJoint.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            vertexTorsionExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            vertexTorsionExpansionJointEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(torsionExpansionJoint));
            vertexTorsionExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(torsionExpansionJoint));

            return vertexTorsionExpansionJointEntity;
        }
    }
}