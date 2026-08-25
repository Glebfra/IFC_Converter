using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces.Relations
{
    public interface IIfcRelDefinesByPropertiesBuilder<out T> where T : IIfcRelDefinesByProperties
    {
        public IReadOnlyCollection<IIfcObjectDefinition> RelatedObjects { get; }
        public IIfcPropertySetDefinitionSelect RelatingPropertyDefinition { get; }

        public T? Instance { get; }
        public T CreateInstance(IModel model);

        public void AddPropertySet(IIfcPropertySet propertySet);
        public void AddRelatedObject(IIfcObjectDefinition relatedObject);
    }
}