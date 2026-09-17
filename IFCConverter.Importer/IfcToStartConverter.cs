using System.Reflection;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Importer.Pipeline;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Utils.Pipeline;
using Xbim.Common;
using IfcProject = IFCConverter.IFC.API.IfcProject;

namespace IFCConverter.Importer
{
    public class IfcToStartConverter
    {
        private readonly ImportDataContainer _importDataContainer;
        private readonly Logger _logger = Logger.GetInstance();

        private readonly IfcToStartPipeline _pipeline = new IfcToStartPipeline();

        public IfcToStartConverter(ImportDataContainer importDataContainer)
        {
            _importDataContainer = importDataContainer;
        }

        public void Convert(IStartDocument startDocument)
        {
            _logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            using (IIfcProject project = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                IModel model = project.Model;
                using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
                {
                    _pipeline.Execute(model, startProject);
                }
            }
        }
    }
}