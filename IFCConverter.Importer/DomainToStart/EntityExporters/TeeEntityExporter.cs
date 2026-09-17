using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class TeeEntityExporter : IEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Tee;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            Tee tee = (Tee)entity;

            StartWeldedTeeEntity startTee = new StartWeldedTeeEntity();
            startTee.Position = tee.Position;
            startTee.HeaderLength.CreateFromSI((tee.PortB.Position - tee.PortA.Position).L2Norm());
            startTee.CrotchHeight.CreateFromSI((tee.PortC.Position - tee.Position).L2Norm() - tee.PortA.Metadata.Diameter / 2);

            context.Register(tee, startTee);
        }
    }
}