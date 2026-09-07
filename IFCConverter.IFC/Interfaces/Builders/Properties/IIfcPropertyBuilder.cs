using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.IFC.Interfaces.Properties
{
    public interface IIfcPropertyBuilder<out T> where T : IIfcProperty
    {
        bool IsCreated { get; }

        IfcIdentifier Name { get; }
        IfcText Description { get; }
        T Instance { get; }

        T CreateInstance(IModel model);
    }
}