using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities
{
    public class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
    {
        protected override IfcIdentifier Tag { get; set; } = "Fabricated Tee";
    
        public readonly double Length;
        public readonly double Height;

        public IfcFabricatedTeeEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] ifcPipeEntities) 
            : base(teeEntity, nodeEntity, ifcPipeEntities)
        {
            Length = teeEntity.HeaderLength;
            Height = teeEntity.BranchHeight + _branchPipes[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeFitting = CreateTeeEntity(model, Length, Height);
            return _pipeFitting;
        }
    }
}