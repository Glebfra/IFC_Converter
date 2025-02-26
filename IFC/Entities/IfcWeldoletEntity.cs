using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities
{
    public class IfcWeldoletEntity : IfcAbstractTeeEntity
    {
        protected override IfcIdentifier Tag { get; set; } = "Weldolet";
    
        private double Length;
        private double Height;
    
        public IfcWeldoletEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcPipeEntity[] ifcPipeEntities) 
            : base(teeEntity, nodeEntity, ifcPipeEntities)
        {
            Length = _headPipe.Diameter;
            Height = _branchPipes[0].Diameter / 2 + teeEntity.BranchHeight;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeFitting = CreateTeeEntity(model, Length, Height);
            return _pipeFitting;
        }
    }
}