using IFC.Entities.Abstract;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }

        public IfcFabricatedTeeEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, abstractSegmentEntities)
        {
            Length = startTeeEntity.HeaderLength.SIProperty;
            Height = startTeeEntity.BranchHeight.SIProperty + _BranchPipes[0].OuterDiameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateTeeEntity(model);
            AddProperties(model, pipeFitting);
            ClipPipes();
            return pipeFitting;
        }
    }
}