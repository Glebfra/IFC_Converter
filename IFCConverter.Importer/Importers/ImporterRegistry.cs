using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Interfaces;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.Interfaces;
using Utils;

namespace IFCConverter.Importer.Importers
{
    internal class ImporterRegistry
    {
        private static readonly Lazy<ImporterRegistry> _instance = new(() => new ImporterRegistry());

        private readonly List<ImporterRegistration> _registrations = new();

        private ImporterRegistry()
        {
            RegisterAll();
        }

        public static ImporterRegistry GetInstance()
        {
            return _instance.Value;
        }

        [Pure]
        public IImporter CreateImporter(IIfcProject ifcProject)
        {
            ImporterRegistration? match = _registrations
                .Where(r => r.ImporterFilter.IsMatch(ifcProject))
                .OrderByDescending(r => r.Priority)
                .FirstOrDefault();
            if (match == null)
                throw new InvalidOperationException("No matching importer found.");

            return ParameterlessConstructorRegistry<IImporter>.Create(match.ImporterType);
        }

        private void RegisterAll()
        {
            IEnumerable<Type> runtimeTypes = AttributeFinder.GetClassesWithAttribute<IfcImporterAttribute>();
            foreach (Type runtimeType in runtimeTypes)
            {
                IfcImporterAttribute attribute = runtimeType.GetCustomAttribute<IfcImporterAttribute>();
                if (!typeof(IImporterFilter).IsAssignableFrom(attribute.Filter))
                    continue;
                IImporterFilter importerFilter = ParameterlessConstructorRegistry<IImporterFilter>.Create(attribute.Filter);
                _registrations.Add(new ImporterRegistration(runtimeType, importerFilter, attribute.Priority));
            }
        }
    }
}