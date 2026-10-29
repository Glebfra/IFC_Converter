using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class TeeEntityExporter : IEntityExporter
    {
        private Tee _tee;

        public bool CanExport(Entity entity)
        {
            if (!(entity is Tee tee))
                return false;
            _tee = tee;

            return true;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            StartWeldedTeeEntity startTee = new StartWeldedTeeEntity
            {
                Position = _tee.Position,
                Name = _tee.Metadata.Name
            };
            startTee.HeaderLength.CreateFromSI((_tee.PortB.Position - _tee.PortA.Position).L2Norm());
            startTee.CrotchHeight.CreateFromSI((_tee.PortC.Position - _tee.Position).L2Norm() - _tee.PortA.Metadata.Diameter / 2);

            context.Register(_tee, startTee);
        }
    }
}