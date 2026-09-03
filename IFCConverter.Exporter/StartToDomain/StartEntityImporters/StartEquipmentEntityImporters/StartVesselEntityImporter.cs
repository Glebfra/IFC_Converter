using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Equipments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartEquipmentEntityImporters
{
    internal sealed class StartVesselEntityImporter : IStartEquipmentEntityImporter
    {
        public bool CanImport(StartAbstractEquipmentEntity source)
        {
            return source is StartVesselEntity;
        }

        public void Import(StartAbstractEquipmentEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartVesselEntity start = (StartVesselEntity)source;

            Equipment equipment = new Equipment(EntityId.New())
            {
                Position = start.Position
            };
            
            model.Add(equipment);
            context.Register(source, equipment);
        }
    }
}