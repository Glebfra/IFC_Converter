using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Joints;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartExpansionJointEntityImporter : IStartEntityImporter
    {
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractExpansionJointEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractExpansionJointEntity start = (StartAbstractExpansionJointEntity)source;

            Joint joint = new Joint(EntityId.New())
            {
                Position = start.Position,
                Length = start.Length.SIProperty
            };
            
            model.Add(joint);
            context.Register(start, joint);
        }
    }
}