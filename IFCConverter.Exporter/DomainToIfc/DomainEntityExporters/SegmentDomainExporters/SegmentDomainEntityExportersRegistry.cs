using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.API;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.SegmentDomainExporters
{
    internal sealed class SegmentDomainEntityExportersRegistry : ReflectionRegistry<ISegmentDomainEntityExporter>, ISegmentDomainEntityExportersRegistry
    {
        private readonly IDictionary<StartElementTypeEnum, ISegmentDomainEntityExporter> _exportersMapCache =
            new Dictionary<StartElementTypeEnum, ISegmentDomainEntityExporter>();

        public SegmentDomainEntityExportersRegistry() : base(typeof(SegmentDomainEntityExportersRegistry).Assembly)
        {
        }

        public bool TryResolve(Entity entity, out ISegmentDomainEntityExporter exporter)
        {
            exporter = null;

            if (!Enum.TryParse(entity.Metadata.Type, out StartElementTypeEnum type))
                return false;
            if (_exportersMapCache.TryGetValue(type, out exporter))
                return true;
            if (TryResolve(exp => exp.CanExport(type), out exporter))
            {
                _exportersMapCache.Add(type, exporter);
                return true;
            }

            return false;
        }
    }
}