using IFC.Entities.Abstract;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcStubInEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }

        public IfcStubInEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, abstractSegmentEntities)
        {
            Length = _HeadPipe.OuterDiameter;
            Height = _BranchPipes[0].OuterDiameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateTeeEntity(model);
            AddProperties(model, pipeFitting);
            return pipeFitting;
        }
    }
}