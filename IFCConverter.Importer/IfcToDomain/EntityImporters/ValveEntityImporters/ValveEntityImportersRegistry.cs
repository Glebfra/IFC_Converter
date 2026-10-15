using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal sealed class ValveEntityImportersRegistry : ReflectionRegistry<IValveEntityImporter>, IValveEntityImportersRegistry
    {
        public ValveEntityImportersRegistry() : base(typeof(ValveEntityImportersRegistry).Assembly)
        {
        }

        public IValveEntityImporter Resolve(IIfcProduct product)
        {
            return Resolve(importer => importer.CanImport(product));
        }

        public bool TryResolve(IIfcProduct product, out IValveEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product), out importer);
        }
    }
}