using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

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

            FixedVector<Dim3> directionToMinSegment = minSegment.GetProjectionFromPoint(reducer.Position);
            FixedVector<Dim3> directionToMaxSegment = maxSegment.GetProjectionFromPoint(maxSegment.GetNearestPosition(reducer.Position)).Normalize();
            FixedVector<Dim3> maxPosition = reducer.Position + directionToMaxSegment * reducer.Length;

            reducer.PortA.SetGeometry(reducer.Position, directionToMinSegment);
            reducer.PortB.SetGeometry(maxPosition, directionToMaxSegment);

            reducer.PortA.Metadata.Diameter = DiameterFinder.GetDiameter(minSegment, model, context);
            reducer.PortB.Metadata.Diameter = DiameterFinder.GetDiameter(maxSegment, model, context);
        }
    }
}