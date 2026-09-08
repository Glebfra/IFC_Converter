using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.Entities.Equipments;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Exporter.StartToDomain.PortResolvers
{
    internal sealed class EquipmentPortResolver : IPortResolver
    {
        private const double DiameterToLengthFactor = 0.3;
        
        public bool CanResolve(IStartEntity source)
        {
            return source is StartAbstractEquipmentEntity && !(source is StartPumpApi610Entity);
        }

        public void Resolve(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            if (!context.TryGetEntityId(source, out EntityId id))
                return;

            StartAbstractEquipmentEntity start = (StartAbstractEquipmentEntity)source;
            Equipment equipment = (Equipment)model.GetEntity(id);
            
            IStartSegmentEntity[] startSegments = source.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            if (startSegments.Length != 2)
                throw new InvalidOperationException($"Equipment '{equipment.Id}' must have exactly two connected segments");

            double[] diameters = startSegments.Select(segment => DiameterFinder.GetDiameter(segment, model, context)).ToArray();
            double diameter = diameters.Max();
            double length = diameter * DiameterToLengthFactor;

            Vector<double> position = equipment.Position;
            Vector<double>[] directions = startSegments.Select(segment => segment.GetProjectionFromPoint(position)).ToArray();
            Vector<double>[] portPositions = directions.Select(direction => position + direction * length / 2).ToArray();
            
            equipment.PortA.SetGeometry(portPositions[0], directions[0]);
            equipment.PortB.SetGeometry(portPositions[1], directions[1]);

            equipment.PortA.Metadata.Diameter = diameters[0];
            equipment.PortB.Metadata.Diameter = diameters[1];
        }
    }
}