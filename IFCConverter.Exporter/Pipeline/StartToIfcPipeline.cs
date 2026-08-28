using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Utils.Diagnostics;
using IFCConverter.Start.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class StartToIfcPipeline
    {
        private readonly DomainToIfcPipeline _domainToIfcPipeline = new DomainToIfcPipeline();

        private readonly Logger _logger = Logger.GetInstance();
        private readonly StartToDomainPipeline _startToDomainPipeline = new StartToDomainPipeline();

        public void Execute(IReadOnlyCollection<IStartEntity> source, IModel ifcModel, Action<IIfcProduct> addProduct)
        {
            _logger.Info($"Starting: '{nameof(StartToIfcPipeline)}'.");

            EngineeringModel domain = _startToDomainPipeline.Execute(source);
            _domainToIfcPipeline.Execute(domain, ifcModel, addProduct);

            _logger.Info($"Finished '{nameof(StartToIfcPipeline)}'.");
        }
    }
}