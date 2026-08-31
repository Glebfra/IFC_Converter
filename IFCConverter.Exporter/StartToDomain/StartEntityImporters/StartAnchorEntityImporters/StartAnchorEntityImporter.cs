using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Anchors;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartAnchorEntityImporters
{
    internal sealed class StartAnchorEntityImporter : IStartAnchorEntityImporter
    {
        public bool CanImport(StartAbstractAnchorEntity start)
        {
            return !(start is StartNonstandardAnchorEntity);
        }

        public void Import(StartAbstractAnchorEntity start, EngineeringModel model, StartMappingContext context)
        {
            Anchor anchor = new Anchor(EntityId.New())
            {
                Position = start.Position
            };

            model.Add(anchor);
            context.Register(start, anchor);
        }
    }
}