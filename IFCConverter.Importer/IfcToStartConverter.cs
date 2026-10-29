using System.Reflection;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Importer.Pipeline;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using IfcProject = IFCConverter.IFC.API.IfcProject;

namespace IFCConverter.Importer
{
    public class IfcToStartConverter
    {
        private readonly ILoggerFactory _factory;
        private readonly ImportDataContainer _importDataContainer;

        private readonly IfcToStartPipeline _pipeline = new IfcToStartPipeline();

        public IfcToStartConverter(ImportDataContainer importDataContainer, ILoggerFactory factory)
        {
            _importDataContainer = importDataContainer;
            _factory = factory;
        }

        public void Convert(IStartDocument startDocument)
        {
            ILogger logger = _factory.CreateLogger<IfcToStartConverter>();
            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            logger.LogInformation("{name} v.{version}", assembly.Name, assembly.Version);

            using (IIfcProject project = IfcProject.OpenProject(_importDataContainer.InputFilePath))
            {
                IModel model = project.Model;
                using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
                {
                    _pipeline.Execute(model, startProject, _factory);
                }
            }
        }
    }
}