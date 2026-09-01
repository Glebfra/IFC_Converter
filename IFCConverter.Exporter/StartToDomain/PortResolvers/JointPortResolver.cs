using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Joints;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using MathNet.Numerics.LinearAlgebra;

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
            
            Vector<double> position = joint.Position;
            Vector<double>[] directions = segments.Select(segment => segment.GetProjectionFromPoint(position)).ToArray();
            Vector<double>[] portPositions = directions.Select(direction => position + direction * joint.Length / 2).ToArray();
            
            joint.PortA.SetGeometry(portPositions[0], directions[0]);
            joint.PortB.SetGeometry(portPositions[1], directions[1]);
            
            joint.PortA.Metadata.Diameter = segments[0].Diameter.SIProperty;
            joint.PortB.Metadata.Diameter = segments[1].Diameter.SIProperty;
        }
    }
}