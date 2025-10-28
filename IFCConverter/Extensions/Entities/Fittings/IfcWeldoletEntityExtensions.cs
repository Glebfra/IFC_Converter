using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcWeldoletEntityExtensions
    {
        public static IfcWeldoletEntity CreateFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = headPipe.Diameter;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;

            IfcWeldoletEntity weldoletEntity = new IfcWeldoletEntity(
                teeEntity.Name,
                teeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            weldoletEntity.ConnectedEntities.AddRange(segmentEntities);
            weldoletEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            weldoletEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            weldoletEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return weldoletEntity;
        }
    }
}