using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
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
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();

            IfcFabricatedTeeEntity fabricatedTeeEntity = new IfcFabricatedTeeEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            fabricatedTeeEntity.ConnectedEntities.AddRange(segmentEntities);
            fabricatedTeeEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            fabricatedTeeEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            fabricatedTeeEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return fabricatedTeeEntity;
        }
    }
}