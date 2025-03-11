using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings
{
    public class IfcSweepoletEntity : IfcAbstractTeeEntity
    {
        private double Length;
        private double Height;
    
        public IfcSweepoletEntity(StartTeeEntity teeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(teeEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            Length = _headPipe.Diameter;
            Height = _branchPipes[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeFitting = CreateTeeEntity(model, Length, Height);
            return _pipeFitting;
        }
    }
}