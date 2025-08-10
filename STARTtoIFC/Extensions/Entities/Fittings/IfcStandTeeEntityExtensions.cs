using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using Start.Entities.Fittings;
using STARTtoIFC.Extensions.PropertySets;
using STARTtoIFC.Extensions.Tools;
using Xbim.Common.Geometry;

namespace STARTtoIFC.Extensions.Entities.Fittings
{
    #if NEW
    
    internal static class IfcStandTeeEntityExtensions
    {
        public static IfcStandTeeEntity CreateFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;

            IfcStandTeeEntity standTeeEntity = new IfcStandTeeEntity(
                teeEntity.Name,
                teeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            standTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            standTeeEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(teeEntity));
            standTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunctionExtensions.CreateFromStart(teeEntity));
            standTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(teeEntity));

            return standTeeEntity;
        }
    }
    
    #endif
}