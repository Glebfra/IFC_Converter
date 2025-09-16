using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
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
            universalExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(universalExpansion));
            universalExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(universalExpansion));

            return universalExpansionJointEntity;
        }
    }
}