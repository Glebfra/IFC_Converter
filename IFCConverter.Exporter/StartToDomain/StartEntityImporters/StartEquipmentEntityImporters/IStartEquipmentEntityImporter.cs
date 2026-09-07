using IFCConverter.Domain;
using IFCConverter.Start.Entities.Equipments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartEquipmentEntityImporters
{
    internal interface IStartEquipmentEntityImporter
    {
        bool CanImport(StartAbstractEquipmentEntity source);
        void Import(StartAbstractEquipmentEntity source, EngineeringModel model, StartMappingContext context);
    }
}