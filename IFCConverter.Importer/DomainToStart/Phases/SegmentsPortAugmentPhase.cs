using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using IFCConverter.Importer.Attributes;

namespace IFCConverter.Importer.DomainToStart.Phases
{
    [DomainToStartPhase(1, typeof(DomainAugmentPhase))]
    internal sealed class SegmentsPortAugmentPhase : IDomainToStartPhase
    {
        public void Execute(EngineeringModel model, ExportContext context)
        {
            foreach (Connection connection in model.Connections)
            {
                if (connection.Type != ConnectionType.PipeToFitting)
                    continue;

                EntityId portAEntityId = connection.PortA.Owner;
                EntityId portBEntityId = connection.PortB.Owner;

                if (model.GetEntity(portAEntityId) is AbstractSegment)
                {
                    AbstractFitting fitting = (AbstractFitting)model.GetEntity(portBEntityId);
                    connection.PortA.SetGeometry(fitting.Position, connection.PortA.Direction);
                }
                else if (model.GetEntity(portBEntityId) is AbstractSegment)
                {
                    AbstractFitting fitting = (AbstractFitting)model.GetEntity(portAEntityId);
                    connection.PortB.SetGeometry(fitting.Position, connection.PortB.Direction);
                }
            }
        }
    }
}