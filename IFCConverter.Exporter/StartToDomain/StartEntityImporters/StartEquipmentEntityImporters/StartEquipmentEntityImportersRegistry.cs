using IFCConverter.Start.Entities.Equipments;
using IFCConverter.Utils.Reflection;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartEquipmentEntityImporters
{
    internal sealed class StartEquipmentEntityImportersRegistry : ReflectionRegistry<IStartEquipmentEntityImporter>, IStartEquipmentEntityImportersRegistry
    {
        public StartEquipmentEntityImportersRegistry() : base(typeof(StartEquipmentEntityImportersRegistry).Assembly)
        {
        }

        public IStartEquipmentEntityImporter Resolve(StartAbstractEquipmentEntity source)
        {
            return Resolve(importer => importer.CanImport(source));
        }

        public bool TryResolve(StartAbstractEquipmentEntity source, out IStartEquipmentEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(source), out importer);
        }
    }
}