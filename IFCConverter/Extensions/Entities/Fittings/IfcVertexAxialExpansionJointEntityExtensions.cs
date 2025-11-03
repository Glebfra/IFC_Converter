using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexAxialExpansionJointEntityExtensions
    {
        public static IfcVertexAxialExpansionJointEntity CreateFromStart(StartAxialExpansionJointEntity expansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = expansionJoint.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;
            
            string name = expansionJoint.Name;
            string type = expansionJoint.Type.ToString();

            IfcVertexAxialExpansionJointEntity axialExpansionJointEntity = new IfcVertexAxialExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );
            
            axialExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            axialExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(expansionJoint));
            axialExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(expansionJoint));

            return axialExpansionJointEntity;
        }
    }
}