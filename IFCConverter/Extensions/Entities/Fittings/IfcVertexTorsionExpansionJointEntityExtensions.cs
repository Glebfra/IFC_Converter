using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexTorsionExpansionJointEntityExtensions
    {
        public static IfcVertexTorsionExpansionJointEntity CreateFromStart(StartTorsionExpansionJointEntity torsionExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double diameter = segmentEntities[0].Diameter;
            double length = torsionExpansionJoint.Length.SIProperty;
            
            string name = torsionExpansionJoint.Name;
            string type = torsionExpansionJoint.Type.ToString();

            IfcVertexTorsionExpansionJointEntity vertexTorsionExpansionJointEntity = new IfcVertexTorsionExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            vertexTorsionExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            vertexTorsionExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(torsionExpansionJoint));
            vertexTorsionExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(torsionExpansionJoint));

            return vertexTorsionExpansionJointEntity;
        }
    }
}