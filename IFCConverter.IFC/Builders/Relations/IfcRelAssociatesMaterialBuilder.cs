using System.Collections.Generic;
using IFCConverter.IFC.Interfaces.Relations;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Builders.Relations
{
    public class IfcRelAssociatesMaterialBuilder<T> : IIfcRelAssociatesMaterialBuilder<T>
        where T : IIfcRelAssociatesMaterial, IInstantiableEntity
    {
        private IIfcMaterial _material;
        private readonly List<IIfcDefinitionSelect> _relatedObjects = new List<IIfcDefinitionSelect>();
        
        public IIfcMaterialSelect RelatingMaterial => _material;
        public IReadOnlyCollection<IIfcDefinitionSelect> RelatedObjects => _relatedObjects;

        public T Instance { get; private set; }

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