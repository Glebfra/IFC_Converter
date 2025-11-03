using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
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
            
            string name = axialExpansionJoint.Name;
            string type = axialExpansionJoint.Type.ToString();

            IfcAxialExpansionJointEntity ifcAxialExpansionJointEntity = new IfcAxialExpansionJointEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                diameter,
                numSegments
            );

            ifcAxialExpansionJointEntity.ConnectedEntities.AddRange(segmentEntities);
            ifcAxialExpansionJointEntity.PropertySets.Add(Pset_Start.CreateFromStart(axialExpansionJoint));
            ifcAxialExpansionJointEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(axialExpansionJoint));
            
            return ifcAxialExpansionJointEntity;
        }
    }
}