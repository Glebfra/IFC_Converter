using IFCConverter.IFC.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Builders.Elements
{
    public class IfcPumpBuilder<T> : IfcFlowMovingDeviceBuilder<T>, IIfcPumpBuilder<T>
        where T : IfcPump
    {
        public IfcPumpTypeEnum PredefinedType { get; }
        
        public IfcPumpBuilder(IfcLabel name, IfcIdentifier tag, IfcPumpTypeEnum predefinedType) : base(name, tag)
        {
            PredefinedType = predefinedType;
        }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.PredefinedType = PredefinedType;

            return instance;
        }
    }
}