using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFCConverter.Extensions.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcFabricatedTeeEntityExtensions
    {
        public static IfcFabricatedTeeEntity CreateFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);

            double length = teeEntity.HeaderLength.SIProperty;
            double height = teeEntity.BranchHeight.SIProperty + branchPipes[0].Diameter / 2;

            IfcFabricatedTeeEntity fabricatedTeeEntity = new IfcFabricatedTeeEntity(
                teeEntity.Name,
                teeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            fabricatedTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            fabricatedTeeEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(teeEntity));
            fabricatedTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunctionExtensions.CreateFromStart(teeEntity));
            fabricatedTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(teeEntity));

            return fabricatedTeeEntity;
        }
    }
}