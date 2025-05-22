using IFC.Entities.Abstract;
using IFC.Entities.Abstract.Segments;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcWeldoletEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }
    
        public IfcWeldoletEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, abstractSegmentEntities)
        {
            Length = _HeadPipe.Diameter;
            Height = _BranchPipes[0].Diameter / 2 + startTeeEntity.BranchHeight.SIProperty;
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