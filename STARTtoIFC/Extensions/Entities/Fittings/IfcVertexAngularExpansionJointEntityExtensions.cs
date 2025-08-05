using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.Vertex;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
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

            return angularExpansionJointEntity;
        }
    }
    
    #endif
}