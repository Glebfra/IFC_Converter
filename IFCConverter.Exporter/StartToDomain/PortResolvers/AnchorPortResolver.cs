using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class AnchorPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractAnchorEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Anchor anchor = (Anchor)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            double diameter = DiameterFinder.GetMaxDiameter(segments, model, context);

            Vector<double> position = anchor.Position;
            Vector<double> direction = CalculateDirection(source, segments, position);

            anchor.Port.SetGeometry(position, direction);
            anchor.Port.Metadata.Diameter = diameter;
        }

        private static Vector<double> CalculateDirection(IStartEntity source, IStartSegmentEntity[] segments, Vector<double> position)
        {
            switch (source)
            {
                case StartFixedAnchorEntity _:
                    return segments.First().GetProjectionFromPoint(position).Normalize(2);
                case StartAbstractAnchorEntity _:
                    return VectorExtensions.Z;
            }
            
            throw new InvalidOperationException($"Cannot calculate direction for {source}");
        }
    }
}