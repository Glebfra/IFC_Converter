using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    internal static class IfcVertexAxialExpansionJointEntityExtensions
    {
        public static IfcVertexAxialExpansionJointEntity CreateFromStart(StartAxialExpansionJointEntity expansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = expansionJoint.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;

            IfcVertexAxialExpansionJointEntity axialExpansionJointEntity = new IfcVertexAxialExpansionJointEntity(
                expansionJoint.Name,
                expansionJoint.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            axialExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            axialExpansionJointEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(expansionJoint));
            axialExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(expansionJoint));

            return axialExpansionJointEntity;
        }
    }
}