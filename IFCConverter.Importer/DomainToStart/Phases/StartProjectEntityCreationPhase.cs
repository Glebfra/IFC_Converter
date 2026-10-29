using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;
using Microsoft.Extensions.Logging;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(EntityExportPhase))]
    public class StartProjectEntityCreationPhase : IDomainToStartPhase
    {
        private const double DoubleTolerance = 1e-3;
        private readonly StartNodeRegistry _nodeRegistry;

        public StartProjectEntityCreationPhase()
        {
            _nodeRegistry = new StartNodeRegistry(DoubleTolerance);
        }

        public void Execute(EngineeringModel model, ExportContext context, ILoggerFactory factory)
        {
            ILogger logger = factory.CreateLogger<StartProjectEntityCreationPhase>();
            IStartProject project = context.StartProject;

            foreach (IStartEntity startEntity in context.StartEntities)
            {
                logger.LogInformation("Adding {startEntity} with name {name} to start project", startEntity.GetType().Name, startEntity.Name);
                Entity entity = model.GetEntity(context.GetEntityId(startEntity));
                FixedVector<Dim3>[] nodePositions = entity.Positions.ToArray();
                logger.LogInformation("Added {count} nodes to {startEntity}", nodePositions.Length, startEntity.GetType().Name);

                StartEntityProxy startEntityProxy = project.AddEntity(startEntity);
                StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(project, nodePositions);
                startEntityProxy.ConnectNodes(nodeProxies);
            }

            project.OnImportFinish();
        }
    }
}