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
    
    internal static class IfcNonStandardTeeEntityExtensions
    {
        public static IfcNonStandardTeeEntity CreateFromStart(StartNonstandardTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;

            IfcNonStandardTeeEntity nonstandardTeeEntity = new IfcNonStandardTeeEntity(
                teeEntity.Name,
                teeEntity.Type.ToString(),
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            nonstandardTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            nonstandardTeeEntity.PropertySets.Add(Pset_StartExtensions.CreateFromStart(teeEntity));
            nonstandardTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunctionExtensions.CreateFromStart(teeEntity));
            nonstandardTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantitiesExtensions.CreateFromStart(teeEntity));

            return nonstandardTeeEntity;
        }
    }
    
    #endif
}