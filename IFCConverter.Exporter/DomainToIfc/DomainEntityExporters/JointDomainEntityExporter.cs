using IFCConverter.Domain.Entities;
using IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters;
using Xbim.Common;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class JointDomainEntityExporter : IDomainEntityExporter
    {
        private readonly IJointDomainEntityExportersRegistry _registry = new JointDomainEntityExportersRegistry();
        
        public bool CanExport(Entity entity)
        {
            return entity is Joint;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Joint joint = (Joint)entity;
            if (_registry.TryResolve(joint, out IJointDomainEntityExporter exporter))
                exporter.Export(joint, model, context);
        }
    }
}