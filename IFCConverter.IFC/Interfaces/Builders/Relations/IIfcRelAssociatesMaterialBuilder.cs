using System.Collections.Generic;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.IFC.Interfaces.Relations
{
    public interface IIfcRelAssociatesMaterialBuilder<out T> where T : IIfcRelAssociatesMaterial
    {
        IIfcMaterialSelect RelatingMaterial { get; }
        IReadOnlyCollection<IIfcDefinitionSelect> RelatedObjects { get; }

        T Instance { get; }
        T CreateInstance(IModel model);

        void AddMaterial(IIfcMaterial material);
        void AddRelatedObject(IIfcDefinitionSelect relatedObject);
    }
}