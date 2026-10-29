using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class ValveEntityExporter : IEntityExporter
    {
        private Valve _valve;

        public bool CanExport(Entity entity)
        {
            if (!(entity is Valve valve))
                return false;
            _valve = valve;

            return true;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            StartValveEntity startValveEntity = new StartValveEntity
            {
                Position = _valve.Position,
                Name = _valve.Metadata.Name
            };
            startValveEntity.Length.CreateFromSI(_valve.Length);

            context.Register(_valve, startValveEntity);
        }
    }
}