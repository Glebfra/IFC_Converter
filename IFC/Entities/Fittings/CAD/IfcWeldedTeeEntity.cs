using IFC.Entities.Abstract;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    public sealed class IfcWeldedTeeEntity : IfcAbstractTeeEntity
    {
        public readonly double Length;
        public readonly double Height;

        public IfcWeldedTeeEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] abstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, abstractSegmentEntities)
        {
            Length = startTeeEntity.HeaderLength;
            Height = startTeeEntity.CrotchHeight + _branchPipes[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateTeeEntity(model, Length, Height);
            AddProperties(model, pipeFitting);
            return pipeFitting;
        }
    }
}