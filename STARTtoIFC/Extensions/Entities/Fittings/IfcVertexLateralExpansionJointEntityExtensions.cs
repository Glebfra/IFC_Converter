using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    internal static class IfcVertexLateralExpansionJointEntityExtensions
    {
        public static IfcVertexLateralExpansionJointEntity CreateFromStart(StartLateralExpansionJointEntity lateralExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = lateralExpansion.Length.SIProperty;
            double diameter = length * 2;

            IfcVertexLateralExpansionJointEntity lateralExpansionJointEntity = new IfcVertexLateralExpansionJointEntity(
                lateralExpansion.Name,
                lateralExpansion.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                angle,
                numSegments
            );
            
            lateralExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            lateralExpansionJointEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(lateralExpansion));
            lateralExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(lateralExpansion));

            return lateralExpansionJointEntity;
        }
    }
}