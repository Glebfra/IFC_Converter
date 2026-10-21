using IFCConverter.Domain;
using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ValveEntityImporters
{
    internal sealed class ValveEntityImportersRegistry : ReflectionRegistry<IValveEntityImporter>, IValveEntityImportersRegistry
    {
        public ValveEntityImportersRegistry() : base(typeof(ValveEntityImportersRegistry).Assembly)
        {
        }

        public IValveEntityImporter Resolve(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            return Resolve(importer => importer.CanImport(product, model, context));
        }

        public bool TryResolve(IIfcProduct product, EngineeringModel model, ImportContext context, out IValveEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product, model, context), out importer);
        }
    }
}