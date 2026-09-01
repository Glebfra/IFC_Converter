using IFCConverter.Domain.Entities;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class JointDomainEntityExportersRegistry : ReflectionRegistry<IJointDomainEntityExporter>, IJointDomainEntityExportersRegistry
    {
        public JointDomainEntityExportersRegistry() : base(typeof(JointDomainEntityExportersRegistry).Assembly)
        {
        }

        public IJointDomainEntityExporter Resolve(Joint joint)
        {
            return Resolve(exporter => exporter.CanExport(joint));
        }

        public bool TryResolve(Joint joint, out IJointDomainEntityExporter exporter)
        {
            return TryResolve(exp => exp.CanExport(joint), out exporter);
        }
    }
}