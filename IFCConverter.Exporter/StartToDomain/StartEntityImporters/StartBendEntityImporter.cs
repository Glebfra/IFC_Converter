using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.API;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartBendEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractBendEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractBendEntity start = (StartAbstractBendEntity)source;
            StartElementTypeEnum startType = start.GetStartElementAttribute().Type;

            Elbow elbow = new Elbow(EntityId.New())
            {
                Position = start.Position,
                Radius = start.Radius.SIProperty
            };

            model.Add(elbow);
            context.Register(source, elbow);
        }
    }
}