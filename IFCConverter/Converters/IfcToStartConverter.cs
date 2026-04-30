using System.Collections.Generic;
using System.Reflection;
using IFCConverter.Converters.Importers;
using IFCConverter.Interfaces;
using IFCConverter.Utils;
using Start.API;
using Start.Interfaces;
using Utils;
using Xbim.Ifc4.Kernel;
using IfcProject = Ifc.API.IfcProject;

namespace IFCConverter.Converters
{
    internal class IfcToStartConverter
    {
        private readonly ImportDataContainer _importDataContainer;
        private readonly Logger _logger = Logger.GetInstance();

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                IEnumerable<IStartEntity> startEntities;
                using (IfcProject ifcProject = IfcProject.OpenProject(_importDataContainer.InputFilePath))
                {
                    ImporterRegistry registry = ImporterRegistry.GetInstance();
                    IImporter importer = registry.CreateImporter(ifcProject);

                    IEnumerable<IfcProduct> products = ifcProject.Model.Instances.OfType<IfcProduct>();
                    startEntities = importer.ImportEntities(products);
                }
            }
        }
    }
}