using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcAxialExpansionJointEntityExtensions
    {
        public static IfcAxialExpansionJointEntity CreateFromStart(StartAxialExpansionJointEntity axialExpansionJoint, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateFittingObjectMatrix(nodeEntity, segmentEntities, out double angle);
            
            double length = axialExpansionJoint.Length.SIProperty;
            double diameter = segmentEntities[0].Diameter;

            IfcAxialExpansionJointEntity ifcAxialExpansionJointEntity = new IfcAxialExpansionJointEntity(
                axialExpansionJoint.Name,
                axialExpansionJoint.Type.ToString(),
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );

            ifcAxialExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcAxialExpansionJointEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(axialExpansionJoint));
            ifcAxialExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(axialExpansionJoint));
            
            return ifcAxialExpansionJointEntity;
        }
    }
}