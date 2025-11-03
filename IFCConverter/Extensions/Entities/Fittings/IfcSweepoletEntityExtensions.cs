using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.PropertySets;
using IFCConverter.Extensions.Tools;
using Start.Entities.Fittings;
using Xbim.Common.Geometry;

namespace IFCConverter.Extensions.Entities.Fittings
{
    internal static class IfcSweepoletEntityExtensions
    {
        public static IfcSweepoletEntity CreateFromStart(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            XbimMatrix3D objectMatrix3D = StartToIfcPlacement.CreateTeeObjectMatrix(nodeEntity, segmentEntities, out double angle, out IfcAbstractSegmentEntity headPipe, out IfcAbstractSegmentEntity[] branchPipes);

            double length = headPipe.Diameter;
            double height = branchPipes[0].Diameter / 2;
            
            string name = teeEntity.Name;
            string type = teeEntity.Type.ToString();
            
            IfcSweepoletEntity sweepoletEntity = new IfcSweepoletEntity(
                StartToIfcNaming.GenerateName(name, type, nodeEntity),
                type,
                objectMatrix3D,
                length,
                branchPipes[0].Diameter,
                headPipe.Diameter,
                height,
                angle
            );
            
            sweepoletEntity.ConnectedEntities.AddRange(segmentEntities);
            sweepoletEntity.PropertySets.Add(Pset_Start.CreateFromStart(teeEntity));
            sweepoletEntity.PropertySets.Add(Pset_PipeFittingTypeJunction.CreateFromStart(teeEntity));
            sweepoletEntity.PropertySets.Add(Qto_PipeFittingBaseQuantities.CreateFromStart(teeEntity));

            return sweepoletEntity;
        }
    }
}