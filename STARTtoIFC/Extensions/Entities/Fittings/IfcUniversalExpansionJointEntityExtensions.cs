using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcUniversalExpansionJointEntityExtensions
    {
        public static IfcUniversalExpansionJointEntity CreateFromStart(StartUniversalExpansionJointEntity universalExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);

            double length = universalExpansion.Length.SIProperty;
            double radius = segmentEntities[0].Diameter / 2;

            IfcUniversalExpansionJointEntity universalExpansionJointEntity = new IfcUniversalExpansionJointEntity(
                universalExpansion.Name,
                universalExpansion.Type.ToString(),
                objectMatrix3D,
                length,
                radius
            );
            
            universalExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);

            return universalExpansionJointEntity;
        }
    }
    
    #endif
}