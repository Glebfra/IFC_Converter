using IFCConverter.Domain;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartEquipmentEntityImporters;
using IFCConverter.Start.Entities.Equipments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters
{
    internal sealed class StartEquipmentEntityImporter : IStartEntityImporter
    {
        private readonly IStartEquipmentEntityImportersRegistry _registry = new StartEquipmentEntityImportersRegistry();
        
        public bool CanImport(IStartEntity source)
        {
            return source is StartAbstractEquipmentEntity;
        }

        public void Import(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            StartAbstractEquipmentEntity start = (StartAbstractEquipmentEntity)source;
            if (_registry.TryResolve(start, out IStartEquipmentEntityImporter importer))
                importer.Import(start, model, context);
        }
    }
}