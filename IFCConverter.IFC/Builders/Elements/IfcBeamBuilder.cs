using IFCConverter.IFC.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.SharedBldgElements;

namespace IFCConverter.IFC.Builders.Elements
{
    public class IfcBeamBuilder<T> : IfcElementBuilder<T>, IIfcBeamBuilder<T>
        where T : IfcBeam
    {
        public IfcBeamBuilder(IfcLabel name, IfcIdentifier tag, IfcBeamTypeEnum predefinedType) : base(name, tag)
        {
            PredefinedType = predefinedType;
        }

        public IfcBeamTypeEnum PredefinedType { get; }

        public override T CreateInstance(IModel model)
        {
            T instance = base.CreateInstance(model);
            instance.PredefinedType = PredefinedType;
            return instance;
        }
    }
}