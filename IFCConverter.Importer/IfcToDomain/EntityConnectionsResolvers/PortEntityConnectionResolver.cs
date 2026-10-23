using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers
{
    internal sealed class PortEntityConnectionResolver : IEntityConnectionResolver
    {
        private const double DoubleTolerance = 1e-3;

        public bool CanResolve()
        {
            return true;
        }

        public void Resolve(EngineeringModel model, ImportContext context)
        {
            List<Port> ports = new List<Port>(model.Ports);
            
            for (int i = 0; i < ports.Count; i++)
            {
                Port firstPort = ports[i];
                Entity firstPortOwner = model.GetEntity(firstPort.Owner);
                
                for (int j = i + 1; j < ports.Count; j++)
                {
                    Port secondPort = ports[j];
                    Entity secondPortOwner = model.GetEntity(secondPort.Owner);
                    
                    if (firstPort.Owner == secondPort.Owner)
                        continue;
                    
                    if (!firstPort.Position.AlmostEqual(secondPort.Position, DoubleTolerance))
                        continue;

                    model.Connect(firstPort, secondPort, ResolveConnectionType(firstPortOwner, secondPortOwner));
                }
            }
        }

        private static ConnectionType ResolveConnectionType(Entity first, Entity second)
        {
            if (first is AbstractAnchor && second is AbstractSegment ||
                first is AbstractSegment && second is AbstractAnchor)
                return ConnectionType.PipeToAnchor;
            if (first is AbstractAnchor && second is AbstractFitting ||
                first is AbstractFitting && second is AbstractAnchor)
                return ConnectionType.FittingToAnchor;
            if (first is AbstractSegment && second is AbstractFitting ||
                first is AbstractFitting && second is AbstractSegment)
                return ConnectionType.PipeToFitting;
            if (first is AbstractSegment && second is AbstractSegment)
                return ConnectionType.PipeToPipe;
            if (first is AbstractFitting && second is AbstractFitting)
                return ConnectionType.FittingToFitting;

            return ConnectionType.Undefined;
        }
    }
}