using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcVertexAngularExpansionJointEntityExtensions
    {
        public static IfcVertexAngularExpansionJointEntity CreateFromStart(StartAngularExpansionJointEntity angularExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = angularExpansion.Length.SIProperty;
            double diameter = length;

            IfcVertexAngularExpansionJointEntity angularExpansionJointEntity = new IfcVertexAngularExpansionJointEntity(
                angularExpansion.Name,
                angularExpansion.Type.ToString(),
                objectMatrix3D,
                length,
                angle,
                diameter,
                numSegments
            );
            
            angularExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            angularExpansionJointEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(angularExpansion));
            angularExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(angularExpansion));

            return angularExpansionJointEntity;
        }
    }
}