using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartTeeEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractTeeEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractTeeEntity start = (StartAbstractTeeEntity)source;

            Tee tee = new Tee(EntityId.New())
            {
                Position = start.Position
            };

            model.Add(tee);
            context.Register(source, tee);
        }
    }
}