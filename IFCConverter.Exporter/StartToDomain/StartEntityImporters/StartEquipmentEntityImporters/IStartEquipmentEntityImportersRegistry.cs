using IFCConverter.Start.Entities.Equipments;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartEquipmentEntityImporters
{
    internal interface IStartEquipmentEntityImportersRegistry
    {
        IStartEquipmentEntityImporter Resolve(StartAbstractEquipmentEntity source);
        bool TryResolve(StartAbstractEquipmentEntity source, out IStartEquipmentEntityImporter importer);
    }
}