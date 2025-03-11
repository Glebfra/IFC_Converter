using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings
{
    public class IfcFabricatedTeeEntity : IfcAbstractTeeEntity
    {
        public readonly double Length;
        public readonly double Height;

        public IfcFabricatedTeeEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(teeEntity, nodeEntity, ifcAbstractSegmentEntities)
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