using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Joints;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class JointPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractExpansionJointEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Joint joint = (Joint)model.GetEntity(context.GetEntityId(source));

            IStartSegmentEntity[] segments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            if (segments.Length != 2)
                throw new InvalidOperationException($"Reducer '{joint.Id}' must have exactly two connected segments");

            FixedVector<Dim3> position = joint.Position;
            FixedVector<Dim3>[] directions = segments.Select(segment => segment.GetProjectionFromPoint(position)).ToArray();
            FixedVector<Dim3>[] portPositions = directions.Select(direction => position + direction * (joint.Length / 2)).ToArray();

            joint.PortA.SetGeometry(portPositions[0], directions[0]);
            joint.PortB.SetGeometry(portPositions[1], directions[1]);

            joint.PortA.Metadata.Diameter = DiameterFinder.GetDiameter(segments[0], model, context);
            joint.PortB.Metadata.Diameter = DiameterFinder.GetDiameter(segments[1], model, context);
        }
    }
}