using Ifc.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;

namespace Ifc.Builders.Properties
{
    public class IfcPropertyBuilder<T> : IIfcPropertyBuilder<T>
        where T : IIfcProperty, IInstantiableEntity
    {
        public IfcPropertyBuilder(IfcIdentifier name, IfcText description)
        {
            Name = name;
            Description = description;
        }

        public bool IsCreated { get; private set; }
        public IfcIdentifier Name { get; }
        public IfcText Description { get; }
        public T? Instance { get; private set; }

        public virtual T CreateInstance(IModel model)
        {
            Instance = model.Instances.New<T>(set =>
            {
                set.Name = Name;
                set.Description = Description;
            });
            IsCreated = true;

            return Instance;
        }
    }
}