using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionAugmenters;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Entities.Proxies
{
    [ProxyEntity(typeof(BoundPointConnectionResolver), 2, typeof(ReducerConnectionAugmenter))]
    internal sealed class ReducerProxy : IFittingProxy
    {
        public double MinDiameter { get; }
        public double MaxDiameter { get; }
        public Vector<double> Direction { get; }
        
        private readonly bool _isEccentric;
        private readonly IReadOnlyList<Vector<double>> _boundPoints;
        private IEnumerable<Vector<double>>? _boundary;

        public ReducerProxy(Vector<double> position, IReadOnlyList<Vector<double>> boundPoints, bool isEccentric,
            double length, double minDiameter, double maxDiameter, Vector<double> direction)
        {
            Position = position;
            _boundPoints = boundPoints;
            _isEccentric = isEccentric;
            Length = length;
            MinDiameter = minDiameter;
            MaxDiameter = maxDiameter;
            Direction = direction;
        }

        public double Length { get; }

        public string? Name { get; set; }
        public Vector<double> Position { get; }

        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();

        public IStartEntity ToStartEntity()
        {
            StartAbstractReducerEntity abstractReducerEntity = _isEccentric
                ? new StartReducerEccentricEntity()
                : new StartReducerConcentricEntity();
            abstractReducerEntity.LengthOfConicalPart.CreateFromSI(Length);
            abstractReducerEntity.Position = Position;

            if (Name != null)
                abstractReducerEntity.Name = Name;

            return abstractReducerEntity;
        }

        [Pure]
        private IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            return _boundPoints;
        }
    }
}