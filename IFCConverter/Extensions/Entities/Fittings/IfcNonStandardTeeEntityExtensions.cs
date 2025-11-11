using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcNonStandardTeeEntityExtensions
    {
        public static IfcNonStandardTeeEntity CreateFromStart(StartNonstandardTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);
            
            double length = teeEntity.HeaderLength.SIProperty;
            double height = branchPipes[0].Diameter / 2 + teeEntity.BranchHeight.SIProperty;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcNonStandardTeeEntity nonstandardTeeEntity = new IfcNonStandardTeeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            nonstandardTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            nonstandardTeeEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            nonstandardTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            nonstandardTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return nonstandardTeeEntity;
        }
    }
}