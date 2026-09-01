using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Segments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters
{
    internal sealed class StartSegmentDefinedDiameterEntityImporter : IStartSegmentEntityImporter
    {
        public bool CanImport(StartAbstractSegmentEntity segment)
        {
            return segment is StartPipeEntity || 
                   segment is StartCylindricalShellEntity || 
                   segment is StartConeElementEntity;
        }

        public void Import(StartAbstractSegmentEntity segment, EngineeringModel model, StartMappingContext context)
        {
            Segment pipe = new Segment(EntityId.New())
            {
                Diameter = segment.Diameter.SIProperty,
            };

            model.Add(pipe);
            context.Register(segment, pipe);
        }
    }
}