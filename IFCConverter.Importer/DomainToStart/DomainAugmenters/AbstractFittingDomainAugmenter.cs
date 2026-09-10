using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Domain.Topology;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Importer.DomainToStart.DomainAugmenters
{
    internal sealed class AbstractFittingDomainAugmenter : IDomainAugmenter
    {
        public bool CanAugment(Entity entity)
        {
            return entity is AbstractFitting;
        }

        public void Augment(Entity entity, EngineeringModel model, ExportContext context)
        {
            AbstractFitting fitting = (AbstractFitting)entity;

            IReadOnlyCollection<Connection> connections = model.GetConnections(entity.Id).ToArray();
            if (connections.Count == entity.Ports.Count)
                return;
            
            foreach (Port port in entity.Ports)
            {
                foreach (Connection connection in connections)
                {
                    if (IsConnectionAdded(connection, port))
                        continue;

                    Segment segment = AddSegment(port, fitting);
                    model.Add(segment);
                }
            }
        }

        private static bool IsConnectionAdded(Connection connection, Port port)
        {
            return connection.PortA.Id == port.Id || connection.PortB.Id == port.Id;
        }

        private static Segment AddSegment(Port port, AbstractFitting fitting)
        {
            Vector<double> projection = port.Position - fitting.Position;
            double length = projection.L2Norm();
            
            Segment segment = new Segment(EntityId.New())
            {
                Diameter = port.Metadata.Diameter,
            };

            Vector<double> startPos = fitting.Position;
            Vector<double> endPos = startPos + length * port.Direction;

            Vector<double>[] positions = new Vector<double>[] { startPos, endPos };
            Vector<double>[] directions = new Vector<double>[] { port.Direction.Negate(), port.Direction };

            int i = 0;
            foreach (Port segmentPort in segment.Ports)
            {
                segmentPort.SetGeometry(positions[i], directions[i]);
                segmentPort.Metadata.Diameter = segment.Diameter;
                i++;
            }

            return segment;
        }
    }
}