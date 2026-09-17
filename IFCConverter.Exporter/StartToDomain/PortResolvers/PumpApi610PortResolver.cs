using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Extensions;
using IFCConverter.Domain.Topology;
using IFCConverter.Start.Entities.Equipments;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class PumpApi610PortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartPumpApi610Entity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            PumpApi610 pump = (PumpApi610)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            Segment[] domainSegments = segments.Select(segment => model.GetEntity(context.GetEntityId(segment))).Cast<Segment>().ToArray();

            FixedVector<Dim3> position = pump.Position;
            Segment[] connectedSegments = domainSegments.Where(segment => segment.IsSegmentContainPoint(position)).ToArray();
            if (connectedSegments.Length != 2)
                throw new InvalidOperationException($"Equipment '{pump.Id}' must have exactly two connected segments");
            ResolvePorts(pump, connectedSegments, position, model, context, pump.PortA, pump.PortB);

            FixedVector<Dim3> secondPosition = pump.SecondPosition;
            Segment[] secondConnectedSegments = domainSegments.Where(segment => segment.IsSegmentContainPoint(secondPosition)).ToArray();
            if (secondConnectedSegments.Length != 2)
                throw new InvalidOperationException($"Equipment '{pump.Id}' must have exactly two connected segments");
            ResolvePorts(pump, secondConnectedSegments, secondPosition, model, context, pump.SecondPortA, pump.SecondPortB);
        }

        private static void ResolvePorts(PumpApi610 pump, Segment[] connectedSegments, FixedVector<Dim3> position, EngineeringModel model,
            StartMappingContext context, Port portA, Port portB)
        {
            double length = connectedSegments.Max(segment => segment.Diameter) / 2;

            FixedVector<Dim3>[] directions = connectedSegments.Select(segment => segment.GetDirectionFromPoint(position)).ToArray();
            FixedVector<Dim3>[] portPosition = directions.Select(direction => position + direction * (length / 2)).ToArray();

            portA.SetGeometry(portPosition[0], directions[0]);
            portB.SetGeometry(portPosition[1], directions[1]);

            portA.Metadata.Diameter =
                DiameterFinder.GetDiameter((IStartSegmentEntity)context.GetStartEntity(connectedSegments[0].Id), model, context);
            portB.Metadata.Diameter =
                DiameterFinder.GetDiameter((IStartSegmentEntity)context.GetStartEntity(connectedSegments[1].Id), model, context);
        }
    }
}