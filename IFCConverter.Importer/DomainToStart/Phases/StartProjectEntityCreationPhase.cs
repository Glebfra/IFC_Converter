using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Importer.Attributes;
using IFCConverter.Start.API;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(EntityExportPhase))]
    public class StartProjectEntityCreationPhase : IDomainToStartPhase
    {
        private const double DoubleTolerance = 1e-3;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);
        private readonly StartNodeRegistry _nodeRegistry;

        public StartProjectEntityCreationPhase()
        {
            _nodeRegistry = new StartNodeRegistry(_comparer);
        }
        
        public void Execute(EngineeringModel model, ExportContext context)
        {
            IStartProject project = context.StartProject;
            
            foreach (IStartEntity startEntity in context.StartEntities)
            {
                Entity entity = model.GetEntity(context.GetEntityId(startEntity));
                Vector<double>[] nodePositions = entity.Positions.ToArray();
                
                StartEntityProxy startEntityProxy = project.AddEntity(startEntity);
                StartEntityProxy[] nodeProxies = _nodeRegistry.GetOrCreateNodes(project, nodePositions);
                startEntityProxy.ConnectNodes(nodeProxies);
            }
            
            project.OnImportFinish();
        }
    }
}