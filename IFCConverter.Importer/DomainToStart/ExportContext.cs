using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Importer.DomainToStart
{
    public sealed class ExportContext
    {
        private readonly Dictionary<EntityId, IStartEntity> _entities = new Dictionary<EntityId, IStartEntity>();
        private readonly Dictionary<IStartEntity, EntityId> _reversedEntities = new Dictionary<IStartEntity, EntityId>();

        public ExportContext(IStartProject startProject)
        {
            StartProject = startProject;
        }

        public IStartProject StartProject { get; }
        public IEnumerable<IStartEntity> StartEntities => _entities.Values;

        public void Register(Entity entity, IStartEntity startEntity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (startEntity == null)
                throw new ArgumentNullException(nameof(startEntity));

            RegisterEntities(entity.Id, startEntity);
        }

        public EntityId GetEntityId(IStartEntity startEntity)
        {
            return _reversedEntities[startEntity];
        }

        public bool TryGetEntityId(IStartEntity startEntity, out EntityId entityId)
        {
            return _reversedEntities.TryGetValue(startEntity, out entityId);
        }

        public IStartEntity GetStartEntity(EntityId id)
        {
            return _entities[id];
        }

        public bool TryGetStartEntity(EntityId id, out IStartEntity startEntity)
        {
            return _entities.TryGetValue(id, out startEntity);
        }

        private void RegisterEntities(EntityId id, IStartEntity startEntity)
        {
            _entities.Add(id, startEntity);
            _reversedEntities.Add(startEntity, id);
        }
    }
}