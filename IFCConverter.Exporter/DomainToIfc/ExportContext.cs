using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.DomainToIfc
{
    public sealed class ExportContext
    {
        private readonly Dictionary<EntityId, IIfcProduct> _products = new Dictionary<EntityId, IIfcProduct>();

        public IReadOnlyCollection<IIfcProduct> Products => _products.Values;

        public void Register(Entity entity, IIfcProduct product)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));
            if (product == null)
                throw new ArgumentNullException(nameof(product));

            if (_products.ContainsKey(entity.Id))
                throw new InvalidOperationException("Domain entity is already exported");

            _products.Add(entity.Id, product);
        }

        public IIfcProduct Get(EntityId entityId)
        {
            IIfcProduct product;
            if (!_products.TryGetValue(entityId, out product))
                throw new KeyNotFoundException($"No IFC product exists for Domain entity {entityId}");

            return product;
        }

        public bool TryGet(EntityId entityId, out IIfcProduct result)
        {
            return _products.TryGetValue(entityId, out result);
        }
    }
}