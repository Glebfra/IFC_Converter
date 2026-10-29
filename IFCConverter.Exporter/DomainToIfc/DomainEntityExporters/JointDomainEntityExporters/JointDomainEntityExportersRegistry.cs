using System;
using System.Collections.Generic;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.API;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class JointDomainEntityExportersRegistry : ReflectionRegistry<IJointDomainEntityExporter>, IJointDomainEntityExportersRegistry
    {
        private readonly IDictionary<StartElementTypeEnum, IJointDomainEntityExporter> _exportersMapCache =
            new Dictionary<StartElementTypeEnum, IJointDomainEntityExporter>();

        public JointDomainEntityExportersRegistry() : base(typeof(JointDomainEntityExportersRegistry).Assembly)
        {
        }

        public bool TryResolve(Joint joint, out IJointDomainEntityExporter exporter)
        {
            exporter = null;

            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
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