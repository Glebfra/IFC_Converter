using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics;
using IFCConverter.Start.Entities;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.ConnectionResolvers
{
    internal sealed class ConnectionResolver : IConnectionResolver
    {
        private const double Tolerance = 1e-6;
        private readonly VectorComparer _vectorComparer = new VectorComparer(Tolerance);

        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;

            Entity entity = model.GetEntity(id);
            IEnumerable<PortId> entityPortIds = entity.Ports.Select(port => port.Id);

            foreach (PortId entityPortId in entityPortIds)
            {
                Port entityPort = model.GetPort(entityPortId);
                IEnumerable<Port> connectiblePorts = model.Ports
                    .Where(port => entityPort.Position.AlmostEqual(port.Position, Tolerance) &&
                                   !port.Equals(entityPort));

                foreach (Port connectiblePort in connectiblePorts)
                {
                    model.Connect(entityPort, connectiblePort, ConnectionType.Undefined);
                }
            }
        }
    }
}