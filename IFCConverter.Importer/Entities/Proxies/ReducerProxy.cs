using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Interfaces;

namespace IFCConverter.Importer.Entities.Proxies
{
    internal sealed class ReducerProxy : IFittingProxy
    {
        private readonly bool _isEccentric;
        private readonly IReadOnlyList<Vector<double>> BoundPoints;
        public Vector<double> Position { get; }
        public double Length { get; }

        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();
        private IEnumerable<Vector<double>>? _boundary;
        
        public string? Name { get; set; }
        
        public ReducerProxy(Vector<double> position, IReadOnlyList<Vector<double>> boundPoints, bool isEccentric, double length)
        {
            Position = position;
            BoundPoints = boundPoints;
            _isEccentric = isEccentric;
            Length = length;
        }

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
            return BoundPoints;
        }
    }
}