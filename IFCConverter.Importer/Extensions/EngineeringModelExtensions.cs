using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.Extensions
{
    public static class EngineeringModelExtensions
    {
        [Pure]
        public static IEnumerable<Connection> GetConnections(this Entity entity, EngineeringModel model)
        {
            return model.GetConnections(entity.Id);
        }

        [Pure]
        public static IEnumerable<Connection> GetConnections(this Port port, EngineeringModel model)
        {
            return model.GetConnections(port.Id);
        }
    }
}