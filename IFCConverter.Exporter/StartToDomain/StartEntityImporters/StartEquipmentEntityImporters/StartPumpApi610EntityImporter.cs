using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Equipments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartEquipmentEntityImporters
{
    internal sealed class StartPumpApi610EntityImporter : IStartEquipmentEntityImporter
    {
        public bool CanImport(StartAbstractEquipmentEntity source)
        {
            return source is StartPumpApi610Entity;
        }

        public void Import(StartAbstractEquipmentEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartPumpApi610Entity start = (StartPumpApi610Entity)source;

            PumpApi610 equipment = new PumpApi610(EntityId.New())
            {
                Position = start.Position,
                SecondPosition = start.SecondPosition
            };
            
            model.Add(equipment);
            context.Register(start, equipment);
        }
    }
}