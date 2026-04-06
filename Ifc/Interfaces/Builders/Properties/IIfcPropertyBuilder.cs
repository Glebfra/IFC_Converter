using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Interfaces
{
    public interface IIfcPropertyBuilder<out T> where T : IIfcProperty
    {
        public bool IsCreated { get; }

        public IfcIdentifier Name { get; }
        public IfcText Description { get; }
        public T? Instance { get; }

        public T CreateInstance(IModel model);
    }
}