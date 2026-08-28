using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartPipeEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractSegmentEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractSegmentEntity start = (StartAbstractSegmentEntity)source;

            PipeSegment pipe = new PipeSegment(EntityId.New())
            {
                Diameter = start.Diameter.SIProperty,
                WallThickness = start.WallThickness.SIProperty
            };

            model.Add(pipe);
            context.Register(source, pipe);
        }
    }
}