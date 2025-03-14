using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings
{
    public sealed class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
    {
        public readonly double Length;
        public readonly double Height;

        public IfcFabricatedTeeEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            Length = startTeeEntity.HeaderLength;
            Height = startTeeEntity.BranchHeight + _branchPipes[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateTeeEntity(model, Length, Height);
            AddProperties(model, pipeFitting);
            return pipeFitting;
        }
    }
}