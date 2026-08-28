using System.Collections.Generic;
using IFCConverter.Exporter.StartToDomain.StartEntityImporters;
using IFCConverter.Utils.Reflection;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class StartEntityImporterRegistry : ReflectionRegistry<IStartEntityImporter>, IStartEntityImporterRegistry
    {
        public StartEntityImporterRegistry() : base(typeof(StartEntityImporterRegistry).Assembly)
        {
        }

        public IStartEntityImporter Resolve(IStartEntity source)
        {
            return Resolve(importer => importer.CanImport(source));
        }

        public IEnumerable<IStartEntityImporter> ResolveAll(IStartEntity source)
        {
            return ResolveAll(importer => importer.CanImport(source));
        }

        public bool TryResolve(IStartEntity source, out IStartEntityImporter importer)
        {
            return TryResolve(imp => imp.CanImport(source), out importer);
        }
    }
}