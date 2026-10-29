using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class DomainEntityExporterRegistry : ReflectionRegistry<IDomainEntityExporter>, IDomainEntityExporterRegistry
    {
        private readonly IDictionary<Type, IDomainEntityExporter> _exportersMapCache = new Dictionary<Type, IDomainEntityExporter>();
        
        public DomainEntityExporterRegistry() : base(typeof(DomainEntityExporterRegistry).Assembly)
        {
        }
        
        public bool TryResolve(Entity entity, out IDomainEntityExporter exporter)
        {
            Type type = entity.GetType();
            if (_exportersMapCache.TryGetValue(type, out exporter))
                return true;

            if (TryResolve(exp => exp.CanExport(entity), out exporter))
            {
                _exportersMapCache.Add(type, exporter);
                return true;
            }

            return false;
        }
    }
}