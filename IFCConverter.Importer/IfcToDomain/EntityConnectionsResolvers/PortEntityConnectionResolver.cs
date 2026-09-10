using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Collections;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers
{
    internal sealed class PortEntityConnectionResolver : IEntityConnectionResolver
    {
        private const double DoubleTolerance = 1e-6;
        private readonly VectorComparer _comparer = new VectorComparer(DoubleTolerance);
        
        public bool CanResolve()
        {
            return true;
        }

        public void Resolve(EngineeringModel model, ImportContext context)
        {
            Dictionary<Vector<double>, List<Port>> ports = new Dictionary<Vector<double>, List<Port>>(_comparer);
            
            foreach (Port port in model.Ports)
            {
                List<Port> portsList = ports.GetOrAdd(port.Position, _ => new List<Port>());
                portsList.Add(port);
            }
            
            foreach (List<Port> portsList in ports.Values)
            {
                if (portsList.Count == 0 || portsList.Count == 1)
                    continue;
                
                Port firstPort = portsList.First();
                Entity firstPortOwner = model.GetEntity(firstPort.Owner);
                
                foreach (Port secondPort in portsList.Skip(1))
                {
                    Entity secondPortOwner = model.GetEntity(secondPort.Owner);
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