using System;
using System.Linq;
using System.Reflection;
using IFCConverter.Exporter.Pipeline;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Pipeline;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using IfcProject = IFCConverter.IFC.API.IfcProject;
using IIfcProject = IFCConverter.IFC.Interfaces.IIfcProject;

namespace IFCConverter.Exporter
{
    public class StartToIfcConverter
    {
        private readonly ExportDataContainer _exportDataContainer;
        private readonly ILoggerFactory _factory;

        private readonly StartToIfcPipeline _pipeline = new StartToIfcPipeline();

        public StartToIfcConverter(ExportDataContainer exportDataContainer, ILoggerFactory factory)
        {
            _exportDataContainer = exportDataContainer;
            _factory = factory;
        }

        public void Convert(StartDocument startDocument)
        {
            ILogger logger = _factory.CreateLogger<StartToIfcConverter>();

            if (startDocument == null)
                throw new ArgumentNullException(nameof(startDocument));

            AssemblyName assembly = Assembly.GetExecutingAssembly().GetName();
            logger.LogInformation("{name} v.{version}", assembly.Name, assembly.Version);

            IStartEntity[] startEntities;
            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                startEntities = startProject.GetStartEntities().ToArray();
                logger.LogInformation("Found {Count} objects", startEntities.Count());

                using (IIfcProject ifcProject = IfcProject.CreateProject(startDocument.GetTitle()))
                {
                    logger.LogInformation("Started {converter}", nameof(StartToIfcConverter));
                    IModel model = ifcProject.Model;
                    _pipeline.Execute(startEntities, model, _factory, product =>
                    {
                        ifcProject.AddEntityRaw((IfcProduct)product);
                    });

                    ifcProject.SaveAs(_exportDataContainer.OutputFilePath);
                }
            }
        }
    }
}