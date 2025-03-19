using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcWeldoletEntity : IfcAbstractTeeEntity
    {
        private double Length;
        private double Height;
    
        public IfcWeldoletEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            Length = _headPipe.Diameter;
            Height = _branchPipes[0].Diameter / 2 + startTeeEntity.BranchHeight;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateTeeEntity(model, Length, Height);
            AddProperties(model, pipeFitting);
            return pipeFitting;
        }
    }
}