using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.ConnectionResolvers;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Importer.Entities.Proxies
{
    [ProxyEntity(typeof(BoundPointConnectionResolver), 2)]
    internal class PipeSegmentProxy : ISegmentProxy
    {
        public double Diameter { get; }
        private IEnumerable<Vector<double>>? _boundary;

        public PipeSegmentProxy(double diameter, double length, Vector<double> position, Vector<double> direction)
        {
            Diameter = diameter;
            Length = length;
            Position = position;
            Direction = direction;
        }

        public string? Name { get; set; }
        public double Length { get; }
        public Vector<double> Position { get; }
        public Vector<double> Direction { get; }
        public Vector<double> EndPosition => Position + Direction * Length;

        public IEnumerable<Vector<double>> Boundary => _boundary ??= GetBoundaryPoints();

        [Pure]
        public IStartEntity ToStartEntity()
        {
            Vector<double> pipeProjection = Direction * Length;

            StartPipeEntity startPipeEntity = new();
            startPipeEntity.Diameter.CreateFromSI(Diameter);
            startPipeEntity.StartPosition = Position;
            startPipeEntity.Projection = pipeProjection;

            if (Name != null)
                startPipeEntity.Name = Name;

            return startPipeEntity;
        }

        [Pure]
        private IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            return new[] { Position, EndPosition };
        }
    }
}