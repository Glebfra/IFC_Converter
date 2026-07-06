using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Elements
{
    public class IfcPipeSegmentBuilder<T> : IfcFlowSegmentBuilder<T>, IIfcPipeSegmentBuilder<T>
        where T : IfcPipeSegment
    {
        public IfcPipeSegmentBuilder(IfcLabel name, IfcIdentifier tag, IfcPipeSegmentTypeEnum type) : base(name, tag)
        {
            PipeSegmentTypeEnum = type;
        }

        public IfcPipeSegmentTypeEnum PipeSegmentTypeEnum { get; }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.PredefinedType = PipeSegmentTypeEnum;
            return instance;
        }
    }
}