using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    public sealed class StartMappingContext
    {
        private readonly Dictionary<IStartEntity, Id<EntityId>> _entities = new Dictionary<IStartEntity, Id<EntityId>>();
        private readonly Dictionary<Id<EntityId>, IStartEntity> _entitiesReversed = new Dictionary<Id<EntityId>, IStartEntity>();

        public void Register(IStartEntity source, Entity target)
        {
            if (source == null)
                throw new ArgumentNullException(nameof(source));
            if (target == null)
                throw new ArgumentNullException(nameof(target));

            if (_entities.ContainsKey(source))
                throw new InvalidOperationException("START entity is already mapped");

            RegisterEntity(source, target.Id);
        }

        public IStartEntity GetStartEntity(Id<EntityId> id)
        {
            IStartEntity result;
            if (!_entitiesReversed.TryGetValue(id, out result))
                throw new KeyNotFoundException("START entity has not been mapped");

            return result;
        }

        public bool TryGetStartEntityId(Id<EntityId> id, out IStartEntity result)
        {
            return _entitiesReversed.TryGetValue(id, out result);
        }

        public Id<EntityId> GetEntityId(IStartEntity source)
        {
            Id<EntityId> result;
            if (!_entities.TryGetValue(source, out result))
                throw new KeyNotFoundException("START entity has not been mapped");

            return result;
        }

        public bool TryGetEntityId(IStartEntity source, out Id<EntityId> result)
        {
            return _entities.TryGetValue(source, out result);
        }

        private void RegisterEntity(IStartEntity source, Id<EntityId> id)
        {
            _entities.Add(source, id);
            _entitiesReversed.Add(id, source);
        }
    }
}