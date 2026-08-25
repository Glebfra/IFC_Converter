using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace Ifc.Interfaces.Relations
{
    public interface IIfcRelAssociatesMaterialBuilder<out T> where T : IIfcRelAssociatesMaterial
    {
        public IIfcMaterialSelect RelatingMaterial { get; }
        public IReadOnlyCollection<IIfcDefinitionSelect> RelatedObjects { get; }
        
        public T? Instance { get; }
        public T CreateInstance(IModel model);
        
        public void AddMaterial(IIfcMaterial material);
        public void AddRelatedObject(IIfcDefinitionSelect relatedObject);
    }
}