using IFC.Entities.Abstract;
using Start.Entities;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFC.Entities
{
    public class IfcSweepoletEntity : IfcAbstractTeeEntity
    {
        public override IfcIdentifier Tag { get; protected set; } = "Sweepolet";
    
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