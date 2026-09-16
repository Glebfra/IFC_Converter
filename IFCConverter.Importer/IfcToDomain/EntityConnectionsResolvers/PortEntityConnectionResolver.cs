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
            foreach (Port firstPort in model.Ports)
            {
                Entity firstPortOwner = model.GetEntity(firstPort.Owner);
                foreach (Port secondPort in model.Ports)
                {
                    if (firstPort == secondPort)
                        continue;

                    Entity secondPortOwner = model.GetEntity(secondPort.Owner);

                    if (firstPort.Position.AlmostEqual(secondPort.Position, DoubleTolerance))
                        model.Connect(firstPort, secondPort, ResolveConnectionType(firstPortOwner, secondPortOwner));
                }
            }
        }

        private static ConnectionType ResolveConnectionType(Entity first, Entity second)
        {
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