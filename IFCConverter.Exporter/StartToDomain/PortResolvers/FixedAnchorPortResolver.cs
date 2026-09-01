using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class FixedAnchorPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartFixedAnchorEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Anchor anchor = (Anchor)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            double diameter = DiameterFinder.GetMaxDiameter(segments, model, context);

            Vector<double> position = anchor.Position;
            Vector<double> direction = segments.First().GetProjectionFromPoint(position).Normalize(2);

            anchor.Port.SetGeometry(position, direction);
            anchor.Port.Metadata.Diameter = diameter;
        }
    }
}