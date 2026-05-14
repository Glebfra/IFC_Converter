using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Interfaces;
using IFCConverter.Attributes;
using IFCConverter.Interfaces;
using Utils;

namespace IFCConverter.Converters.Importers
{
    internal class ImporterRegistry
    {
        public static ImporterRegistry GetInstance() => _instance.Value;
        private static readonly Lazy<ImporterRegistry> _instance =
            new Lazy<ImporterRegistry>(() => new ImporterRegistry());

        private readonly List<ImporterRegistration> _registrations = new List<ImporterRegistration>();

        private ImporterRegistry()
        {
            RegisterAll();
        }

        [Pure]
        public IImporter CreateImporter(IIfcProject ifcProject)
        {
            ImporterRegistration? match = _registrations
                .Where(r => r.Filter.IsMatch(ifcProject))
                .OrderByDescending(r => r.Priority)
                .FirstOrDefault();
            if (match == null)
                throw new InvalidOperationException("No matching importer found.");
            
            return (IImporter)Activator.CreateInstance(match.ImporterType)!;
        }

        private void RegisterAll()
        {
            IEnumerable<Type> runtimeTypes = AttributeFinder.GetClassesWithAttribute<IfcImporterAttribute>();
            foreach (Type runtimeType in runtimeTypes)
            {
                IfcImporterAttribute attribute = runtimeType.GetCustomAttribute<IfcImporterAttribute>();
                if (!typeof(IFilter).IsAssignableFrom(attribute.Filter))
                    continue;
  
                IFilter filter = (IFilter)Activator.CreateInstance(attribute.Filter, new object[] {});
                _registrations.Add(new ImporterRegistration(runtimeType, filter, attribute.Priority));
            }
        }
    }
}