using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class ValvePortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartValveEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;

            Valve valve = (Valve)model.GetEntity(id);
            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            if (segments.Length != 2)
                throw new InvalidOperationException($"Valve '{valve.Id}' must have exactly two segments");

            Vector<double> position = valve.Position;
            Vector<double>[] directions = segments.Select(segment => segment.GetProjectionFromPoint(position).Normalize(2)).ToArray();

            Vector<double>[] portPositions = directions.Select(direction => position + direction * valve.Length / 2).ToArray();

            valve.PortA.SetGeometry(portPositions[0], directions[0]);
            valve.PortB.SetGeometry(portPositions[1], directions[1]);

            valve.PortA.Metadata.Diameter = segments[0].Diameter.SIProperty;
            valve.PortB.Metadata.Diameter = segments[1].Diameter.SIProperty;
        }
    }
}