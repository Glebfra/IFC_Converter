using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class AnchorPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractAnchorEntity && !(source is StartFixedAnchorEntity);
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Anchor anchor = (Anchor)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            double diameter = DiameterFinder.GetMaxDiameter(segments, model, context);

            Vector<double> position = anchor.Position;
            Vector<double> direction = VectorExtensions.Z;

            anchor.Port.SetGeometry(position, direction);
            anchor.Port.Metadata.Diameter = diameter;
        }
    }
}