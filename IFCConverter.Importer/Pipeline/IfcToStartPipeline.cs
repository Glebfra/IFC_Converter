using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;
using Microsoft.Extensions.Logging;
using Xbim.Common;

namespace IFCConverter.Importer.Pipeline
{
    internal sealed class IfcToStartPipeline
    {
        private readonly DomainToStartPipeline _domainToStartPipeline = new DomainToStartPipeline();
        private readonly IfcToDomainPipeline _ifcToDomainPipeline = new IfcToDomainPipeline();

        public void Execute(IModel model, IStartProject startProject, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<IfcToStartPipeline>();
            logger.LogInformation("Starting {pipeline}", nameof(IfcToStartPipeline));
            
            logger.LogInformation("Starting {pipeline}", nameof(IfcToDomainPipeline));
            EngineeringModel domain = _ifcToDomainPipeline.Execute(model, factory);
            logger.LogInformation("Finished {pipeline}", nameof(IfcToDomainPipeline));
            logger.LogInformation(
                "Created {count} domain entities. Created {ports} ports. Created {connections} connections",
                domain.Entities.Count,
                domain.Ports.Count,
                domain.Connections.Count
            );
            
            logger.LogInformation("Starting {pipeline}", nameof(DomainToStartPipeline));
            _domainToStartPipeline.Execute(domain, startProject, factory);
            logger.LogInformation("Finished {pipeline}", nameof(DomainToStartPipeline));
        }
    }
}