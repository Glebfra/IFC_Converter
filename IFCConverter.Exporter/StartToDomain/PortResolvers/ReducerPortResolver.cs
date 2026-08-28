using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class ReducerPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractReducerEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Reducer reducer = (Reducer)model.GetEntity(context.GetEntityId(source));
            StartAbstractReducerEntity start = (StartAbstractReducerEntity)source;

            IStartSegmentEntity[] segments = start.ConnectedEntities.OfType<IStartSegmentEntity>().OrderBy(segment => segment.Diameter).ToArray();
            if (segments.Length != 2)
                throw new InvalidOperationException($"Reducer '{reducer.Id}' must have exactly two connected segments");

            IStartSegmentEntity minSegment = segments[0];
            IStartSegmentEntity maxSegment = segments[1];

            Vector<double> directionToMinSegment = minSegment.GetProjectionFromPoint(reducer.Position);
            Vector<double> directionToMaxSegment = maxSegment.GetProjectionFromPoint(maxSegment.GetNearestPosition(reducer.Position)).Normalize(2);
            Vector<double> maxPosition = reducer.Position + directionToMaxSegment * (double)reducer.Length;

            reducer.PortA.SetGeometry(reducer.Position, directionToMinSegment);
            reducer.PortB.SetGeometry(maxPosition, directionToMaxSegment);

            reducer.PortA.Metadata.Diameter = minSegment.Diameter.SIProperty;
            reducer.PortB.Metadata.Diameter = maxSegment.Diameter.SIProperty;
        }
    }
}