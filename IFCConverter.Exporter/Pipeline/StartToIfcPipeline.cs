using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Pipeline
{
    internal sealed class StartToIfcPipeline
    {
        private readonly DomainToIfcPipeline _domainToIfcPipeline = new DomainToIfcPipeline();
        private readonly StartToDomainPipeline _startToDomainPipeline = new StartToDomainPipeline();

        public void Execute(IReadOnlyCollection<IStartEntity> source, IModel ifcModel, ILoggerFactory factory, Action<IIfcProduct> addProduct)
        {
            ILogger logger = factory.CreateLogger<StartToIfcPipeline>();

            logger.LogInformation("Starting: {name}", nameof(StartToIfcPipeline));

            logger.LogInformation("Starting {name}", nameof(StartToDomainPipeline));
            EngineeringModel domain = _startToDomainPipeline.Execute(source, factory);
            logger.LogInformation("Finished {name}", nameof(StartToDomainPipeline));
            logger.LogInformation("Converted {count} to domain entities", domain.Entities.Count);
            logger.LogInformation("Created {count} ports", domain.Ports.Count);

            logger.LogInformation("Starting {name}", nameof(DomainToIfcPipeline));
            _domainToIfcPipeline.Execute(domain, ifcModel, factory, addProduct);
            logger.LogInformation("Finished {name}", nameof(DomainToIfcPipeline));

            logger.LogInformation("Finished {name}", nameof(StartToIfcPipeline));
        }
    }
}