using IFC.Entities.Abstract;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcNonStandardTeeEntity : IfcAbstractTeeEntity
    {
        public override double Length { get; protected set; }
        public override double Height { get; protected set; }

        private IfcPipeFitting _pipeFitting;
        
        public IfcNonStandardTeeEntity(StartNonstandardTeeEntity nonstandardTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(nonstandardTeeEntity, nodeEntity, abstractSegmentEntities)
        {
            Length = nonstandardTeeEntity.HeaderLength.SIProperty;
            Height = Diameter / 2 + nonstandardTeeEntity.BranchHeight.SIProperty;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            _pipeFitting = CreateTeeEntity(model);
            AddProperties(model, _pipeFitting);
            return _pipeFitting;
        }
    }
}