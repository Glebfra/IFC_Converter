using IFC.Entities.Abstract;
using Start.API;
using Start.Entities;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Fittings.CAD
{
    [IfcEntityType(false, StartElementType.STUB_IN)]
    public sealed class IfcStubInEntity : IfcAbstractTeeEntity
    {
        public readonly double Length;
        public readonly double Height;
    
        public IfcStubInEntity(StartTeeEntity startTeeEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] ifcAbstractSegmentEntities) 
            : base(startTeeEntity, nodeEntity, ifcAbstractSegmentEntities)
        {
            Length = _headPipe.Diameter;
            Height = _branchPipes[0].Diameter / 2;
        }

        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateTeeEntity(model, Length, Height);
            AddProperties(model, pipeFitting);
            return pipeFitting;
        }
    }
}