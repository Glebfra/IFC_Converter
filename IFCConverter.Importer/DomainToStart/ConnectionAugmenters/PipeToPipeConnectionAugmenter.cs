using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Topology;

namespace IFCConverter.Importer.DomainToStart.ConnectionAugmenters
{
    internal sealed class PipeToPipeConnectionAugmenter : IConnectionAugmenter
    {
        public bool CanAugment(Connection connection)
        {
            return connection.Type == ConnectionType.PipeToPipe;
        }

        public void Augment(Connection connection, EngineeringModel model, ExportContext context)
        {
            Port[] ports = connection.Ports.ToArray();
            ports[0].SetGeometry(ports[1].Position, ports[0].Direction);
        }
    }
}