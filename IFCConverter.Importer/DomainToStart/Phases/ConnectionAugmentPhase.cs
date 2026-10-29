using IFCConverter.Domain;
using IFCConverter.Domain.Topology;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.DomainToStart.ConnectionAugmenters;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1)]
    public sealed class ConnectionAugmentPhase : IDomainToStartPhase
    {
        private readonly IConnectionAugmentersRegistry _registry = new ConnectionAugmentersRegistry();

        public void Execute(EngineeringModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<ConnectionAugmentPhase>();

            foreach (Connection connection in model.Connections)
            {
                if (_registry.TryResolve(connection, out IConnectionAugmenter augmenter))
                {
                    logger.LogInformation(
                        "Found connection augmenter {augmenter} for connection {type} with id {id}",
                        augmenter.GetType().Name,
                        connection.Type.ToString(),
                        connection.Id.Value
                    );
                    augmenter.Augment(connection, model, context);
                }
            }
        }
    }
}