using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

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

            FixedVector<Dim3> position = valve.Position;
            FixedVector<Dim3>[] directions = segments.Select(segment => segment.GetProjectionFromPoint(position).Normalize()).ToArray();

            FixedVector<Dim3>[] portPositions = directions.Select(direction => position + direction * (valve.Length / 2)).ToArray();

            valve.PortA.SetGeometry(portPositions[0], directions[0]);
            valve.PortB.SetGeometry(portPositions[1], directions[1]);

            valve.PortA.Metadata.Diameter = DiameterFinder.GetDiameter(segments[0], model, context);
            valve.PortB.Metadata.Diameter = DiameterFinder.GetDiameter(segments[1], model, context);
        }
    }
}