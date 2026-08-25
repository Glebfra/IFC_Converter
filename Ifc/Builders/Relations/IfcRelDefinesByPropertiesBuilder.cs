using System;
using System.Collections.Generic;
using Ifc.Interfaces.Relations;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Relations
{
    public class IfcRelDefinesByPropertiesBuilder<T> : IIfcRelDefinesByPropertiesBuilder<T> 
        where T : IIfcRelDefinesByProperties, IInstantiableEntity
    {
        public IReadOnlyCollection<IIfcObjectDefinition> RelatedObjects => _relatedObjects;
        public IIfcPropertySetDefinitionSelect RelatingPropertyDefinition => _propertySet;

        private IIfcPropertySet _propertySet = null!;
        private List<IIfcObjectDefinition> _relatedObjects = new List<IIfcObjectDefinition>();
        
        public T? Instance { get; private set; }
        
        public T CreateInstance(IModel model)
        {
            if (_propertySet == null)
                throw new NullReferenceException($"{nameof(_propertySet)} is not set. Use {nameof(AddPropertySet)} method to set it");
            if (_relatedObjects.Count == 0)
                throw new NullReferenceException($"{nameof(_relatedObjects)} is empty. Use {nameof(AddRelatedObject)} method to fill it");
            
            Instance = model.Instances.New<T>(properties =>
            {
                properties.Name = _propertySet.Name;
                properties.RelatedObjects.AddRange(_relatedObjects);
                properties.RelatingPropertyDefinition = _propertySet;
            });

            return Instance;
        }

        public void AddPropertySet(IIfcPropertySet propertySet)
        {
            _propertySet = propertySet;
        }

        public void AddRelatedObject(IIfcObjectDefinition relatedObject)
        {
            _relatedObjects.Add(relatedObject);
        }
    }
}