using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain
{
    public sealed class ImportContext
    {
        public ImportType ImportType { get; }
        
        private readonly Dictionary<EntityId, IIfcProduct> _entities = new Dictionary<EntityId, IIfcProduct>();
        private readonly Dictionary<IIfcProduct, EntityId> _entitiesReversed = new Dictionary<IIfcProduct, EntityId>();

        public ImportContext(ImportType importType)
        {
            ImportType = importType;
        }

        public void Register(Entity entity, IIfcProduct product)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (product == null)
                throw new ArgumentNullException(nameof(product));
            
            RegisterEntity(entity.Id, product);
        }

        public EntityId GetEntityId(IIfcProduct product)
        {
            return _entitiesReversed[product];
        }

        public bool TryGetEntityId(IIfcProduct product, out EntityId result)
        {
            return _entitiesReversed.TryGetValue(product, out result);
        }

        public IIfcProduct GetProduct(EntityId id)
        {
            return _entities[id];
        }

        public bool TryGetProduct(IIfcProduct product, out EntityId result)
        {
            return _entitiesReversed.TryGetValue(product, out result);
        }

        private void RegisterEntity(EntityId id, IIfcProduct product)
        {
            _entities.Add(id, product);
            _entitiesReversed.Add(product, id);
        }
    }
}