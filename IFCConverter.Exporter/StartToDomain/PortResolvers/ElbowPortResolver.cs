using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Extensions;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class ElbowPortResolver : IPortResolver
    {
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractBendEntity;
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;
            
            StartAbstractBendEntity start = (StartAbstractBendEntity)source;
            Elbow elbow = (Elbow)model.GetEntity(id);
            
            IStartSegmentEntity[] segments = start.ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .ToArray();

            if (segments.Length != 2)
                throw new InvalidOperationException($"Elbow '{elbow.Id}' must have exactly two connected segments");
            
            IStartSegmentEntity firstSegment = segments[0];
            IStartSegmentEntity secondSegment = segments[1];

            Vector<double> firstDirection = firstSegment.GetProjectionFromPoint(elbow.Position).Normalize(2);
            Vector<double> secondDirection = secondSegment.GetProjectionFromPoint(elbow.Position).Normalize(2);
            double angle = Math.PI - firstDirection.Angle(secondDirection);
            double torusSegmentLength = MathExtensions.CalculateTorusSegmentLength(elbow.Radius, angle);
            
            elbow.PortA.SetGeometry(elbow.Position + firstDirection * torusSegmentLength, firstDirection);
            elbow.PortB.SetGeometry(elbow.Position + secondDirection * torusSegmentLength, secondDirection);
            
            elbow.PortA.Metadata.Diameter = firstSegment.Diameter.SIProperty;
            elbow.PortB.Metadata.Diameter = secondSegment.Diameter.SIProperty;
        }
    }
}