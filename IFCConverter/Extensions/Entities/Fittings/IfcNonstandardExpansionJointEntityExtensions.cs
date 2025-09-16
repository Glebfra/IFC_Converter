using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcNonstandardExpansionJointEntityExtensions
    {
        public static IfcNonstandardExpansionJointEntity CreateFromStart(StartNonstandardExpansionJointEntity nonstandardExpansion, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = nonstandardExpansion.Length.SIProperty;
            double radius = segmentEntities[0].Diameter / 2;

            IfcNonstandardExpansionJointEntity nonstandardExpansionJointEntity = new IfcNonstandardExpansionJointEntity(
                nonstandardExpansion.Name,
                nonstandardExpansion.Type.ToString(),
                objectMatrix3D,
                length,
                radius
            );
            
            nonstandardExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            nonstandardExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(nonstandardExpansion));
            nonstandardExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(nonstandardExpansion));

            return nonstandardExpansionJointEntity;
        }
    }
}