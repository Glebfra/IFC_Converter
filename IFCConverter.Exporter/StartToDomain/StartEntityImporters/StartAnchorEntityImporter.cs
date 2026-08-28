using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartAnchorEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractAnchorEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractAnchorEntity start = (StartAbstractAnchorEntity)source;

            Anchor anchor = new Anchor(EntityId.New())
            {
                Position = start.Position
            };

            model.Add(anchor);
            context.Register(source, anchor);
        }
    }
}