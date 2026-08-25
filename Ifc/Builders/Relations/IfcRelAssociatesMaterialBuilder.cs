using System.Collections.Generic;
using Ifc.Interfaces.Relations;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Builders.Relations
{
    public class IfcRelAssociatesMaterialBuilder<T> : IIfcRelAssociatesMaterialBuilder<T> 
        where T : IIfcRelAssociatesMaterial, IInstantiableEntity
    {
        public IIfcMaterialSelect RelatingMaterial => _material;
        public IReadOnlyCollection<IIfcDefinitionSelect> RelatedObjects => _relatedObjects;

        private IIfcMaterial _material;
        private List<IIfcDefinitionSelect> _relatedObjects;
        
        public T? Instance { get; private set; }
        
        public T CreateInstance(IModel model)
        {
            Instance = model.Instances.New<T>(instance =>
            {
                instance.RelatingMaterial = _material;
                instance.RelatedObjects.AddRange(_relatedObjects);
            });
            return Instance;
        }

        public void AddMaterial(IIfcMaterial material)
        {
            _material = material;
        }

        public void AddRelatedObject(IIfcDefinitionSelect relatedObject)
        {
            _relatedObjects.Add(relatedObject);
        }
    }
}