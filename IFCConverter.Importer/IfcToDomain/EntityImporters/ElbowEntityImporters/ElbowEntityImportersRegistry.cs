using IFCConverter.Utils.Reflection;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityImporters.ElbowEntityImporters
{
    internal sealed class ElbowEntityImportersRegistry : ReflectionRegistry<IElbowEntityImporter>, IElbowEntityImportersRegistry
    {
        public ElbowEntityImportersRegistry() : base(typeof(ElbowEntityImportersRegistry).Assembly)
        {
        }

        public IElbowEntityImporter Resolve(IIfcProduct product)
        {
            return Resolve(importer => importer.CanImport(product));
        }

        public bool TryResolve(IIfcProduct product, out IElbowEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(product), out importer);
        }
    }
}