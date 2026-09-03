using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartBeamEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartBeamEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartBeamEntity start = (StartBeamEntity)source;
            Beam beam = new Beam(EntityId.New())
            {
                Width = start.Width.SIProperty,
                Height = start.Height.SIProperty,
                Diameter = start.Diameter.SIProperty,
                SectionAxisAngle = start.SectionAxisAngle.SIProperty
            };
            
            model.Add(beam);
            context.Register(source, beam);
        }
    }
}