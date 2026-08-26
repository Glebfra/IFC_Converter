using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartReducerEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractReducerEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractReducerEntity start = (StartAbstractReducerEntity)source;

            Reducer reducer = new Reducer(EntityId.New())
            {
                Position = start.Position,
                Length = start.LengthOfConicalPart.SIProperty
            };

            model.Add(reducer);
            context.Register(source, reducer);
        }
    }
}