using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Reflection;
using Ifc.API;
using Ifc.Interfaces;
using IFCConverter.Attributes;
using IFCConverter.Interfaces;
using Utils;

namespace IFCConverter.Converters.Importers
{
    public class ImporterRegistry
    {
        public static ImporterRegistry GetInstance() => _instance.Value;
        private static readonly Lazy<ImporterRegistry> _instance =
            new Lazy<ImporterRegistry>(() => new ImporterRegistry());
        
        private readonly Dictionary<Type, Func<IIfcProject, bool>> _importers =
            new Dictionary<Type, Func<IIfcProject, bool>>();

        private ImporterRegistry()
        {
            RegisterAll();
        }

        [Pure]
        public IImporter CreateImporter(IfcProject ifcProject)
        {
            foreach (KeyValuePair<Type,Func<IIfcProject,bool>> keyValuePair in _importers)
            {
                if (keyValuePair.Value(ifcProject))
                    return (IImporter)Activator.CreateInstance(keyValuePair.Key);
            }
            
            throw new Exception("No matching importer found for the given IFC project.");
        }

        private void RegisterAll()
        {
            IEnumerable<Type> runtimeTypes = AttributeFinder.GetClassesWithAttribute<IfcImporterAttribute>();
            foreach (Type runtimeType in runtimeTypes)
            {
                IfcImporterAttribute attribute = runtimeType.GetCustomAttribute<IfcImporterAttribute>();
                Type filterType = attribute.Filter;

                IFilter? filter = Activator.CreateInstance(filterType, new object[] {}) as IFilter;
                if (filter == null)
                    continue;

                _importers[runtimeType] = filter.IsMatch;
            }
        }
    }
}