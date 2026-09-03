using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (IStartSegmentEntity startSegment in segments)
            {
                if (!context.TryGetEntityId(startSegment, out EntityId id))
                    continue;
                
                switch (model.GetEntity(id))
                {
                    case Beam beam:
                        diameter = Math.Max(diameter, GetMaxDiameter(beam));
                        break;
                    case Segment segment:
                        diameter = Math.Max(diameter, GetDiameter(segment));
                        break;
                }
            }

            return diameter;
        }

        public static double GetDiameter(IStartSegmentEntity segment, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(segment, out EntityId id))
                return 0.0;
            if (model.GetEntity(id) is Beam beam)
                return GetMaxDiameter(beam);
            if (!(model.GetEntity(id) is Segment domainSegment))
                return 0.0;

            return domainSegment.Diameter;
        }

        private static double GetMaxDiameter(Beam beam)
        {
            double[] values = new double[]
            {
                beam.Diameter, beam.Height, beam.Width
            };
            return values.Max();
        }

        private static double GetDiameter(Segment segment)
        {
            return segment.Diameter;
        }
    }
}