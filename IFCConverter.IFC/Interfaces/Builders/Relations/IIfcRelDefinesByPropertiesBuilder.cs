using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Relations
{
    public interface IIfcRelDefinesByPropertiesBuilder<out T> where T : IIfcRelDefinesByProperties
    {
        IReadOnlyCollection<IIfcObjectDefinition> RelatedObjects { get; }
        IIfcPropertySetDefinitionSelect RelatingPropertyDefinition { get; }

        T Instance { get; }
        T CreateInstance(IModel model);

        void AddPropertySet(IIfcPropertySet propertySet);
        void AddRelatedObject(IIfcObjectDefinition relatedObject);
    }
}