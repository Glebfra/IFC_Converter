using System;
using System.Linq;
using System.Reflection;
using IFCConverter.Exporter.Pipeline;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Utils.Pipeline;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Kernel;
using IfcProject = IFCConverter.IFC.API.IfcProject;
using IIfcProject = IFCConverter.IFC.Interfaces.IIfcProject;

namespace IFCConverter.Exporter
{
    public class StartToIfcConverter
    {
        private static readonly Logger Logger = Logger.GetInstance();
        private readonly ExportDataContainer _exportDataContainer;

        private readonly StartToIfcPipeline _pipeline = new StartToIfcPipeline();

        public StartToIfcConverter(ExportDataContainer exportDataContainer)
        {
            _exportDataContainer = exportDataContainer;
        }

        public void Convert(StartDocument startDocument)
        {
            if (startDocument == null)
                throw new ArgumentNullException(nameof(startDocument));

            Logger.System($"STARTtoIFC converter v.{Assembly.GetExecutingAssembly().GetName().Version}");

            IStartEntity[] startEntities;
            using (IStartProject startProject = StartProject.OpenFromDocument(startDocument))
            {
                startEntities = startProject.GetStartEntities().ToArray();
                Logger.Info($"Found {startEntities.Count()} objects");

                using (IIfcProject ifcProject = IfcProject.CreateProject(startDocument.GetTitle()))
                {
                    IModel model = ifcProject.Model;
                    _pipeline.Execute(startEntities, model, product =>
                    {
                        ifcProject.AddEntityRaw((IfcProduct)product);
                    });

                    ifcProject.SaveAs(_exportDataContainer.OutputFilePath);
                }
            }
        }
    }
}