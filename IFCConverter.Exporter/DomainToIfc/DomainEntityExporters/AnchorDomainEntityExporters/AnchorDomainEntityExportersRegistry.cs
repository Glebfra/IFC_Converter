using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.API;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class AnchorDomainEntityExportersRegistry : ReflectionRegistry<IAnchorDomainEntityExporter>, IAnchorDomainEntityExportersRegistry
    {
        private readonly IDictionary<StartElementTypeEnum, IAnchorDomainEntityExporter> _exportersMapCache =
            new Dictionary<StartElementTypeEnum, IAnchorDomainEntityExporter>();

        public AnchorDomainEntityExportersRegistry() : base(typeof(AnchorDomainEntityExportersRegistry).Assembly)
        {
        }

        public bool TryResolve(Anchor anchor, out IAnchorDomainEntityExporter exporter)
        {
            exporter = null;

            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
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