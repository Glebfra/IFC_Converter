using IFCConverter.IFC.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Builders.Elements
{
    public class IfcPipeFittingBuilder<T> : IfcFlowElementBuilder<T>, IIfcPipeFittingBuilder<T>
        where T : IfcPipeFitting
    {
        public IfcPipeFittingBuilder(IfcLabel name, IfcIdentifier tag, IfcPipeFittingTypeEnum type) : base(name, tag)
        {
            PipeFittingType = type;
        }

        public IfcPipeFittingTypeEnum PipeFittingType { get; }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.PredefinedType = PipeFittingType;
            return instance;
        }
    }
}