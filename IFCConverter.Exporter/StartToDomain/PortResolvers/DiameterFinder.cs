using System;
using System.Collections.Generic;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal static class DiameterFinder
    {
        public static double GetMaxDiameter(IReadOnlyCollection<IStartSegmentEntity> segments, EngineeringModel model, StartMappingContext context)
        {
            double diameter = 0.0;
            foreach (IStartSegmentEntity segment in segments)
            {
                if (!context.TryGetEntityId(segment, out EntityId id))
                    continue;
                if (!(model.GetEntity(id) is Segment domainSegment))
                    continue;
                diameter = Math.Max(diameter, domainSegment.Diameter);
            }

            return diameter;
        }

        public static double GetDiameter(IStartSegmentEntity segment, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(segment, out EntityId id))
                return 0.0;
            if (!(model.GetEntity(id) is Segment domainSegment))
                return 0.0;

            return domainSegment.Diameter;
        }
    }
}