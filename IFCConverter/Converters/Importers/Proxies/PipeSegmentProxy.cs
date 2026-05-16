using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal sealed class PipeSegmentProxy : ISegmentProxy
    {
        public readonly double Diameter;
        public double Length { get; }
        public Vector<double> Position { get; }
        public Vector<double> Direction { get; }
        
        public string? Name { get; set; }
        
        private Vector<double> EndPosition => Position + Direction * Length;

        public PipeSegmentProxy(double diameter, double length, Vector<double> position, Vector<double> direction)
        {
            Diameter = diameter;
            Length = length;
            Position = position;
            Direction = direction;
        }

        [Pure]
        public IStartEntity ToStartEntity()
        {
            Vector<double> pipeProjection = Direction * Length;

            StartPipeEntity startPipeEntity = new StartPipeEntity();
            startPipeEntity.Diameter.CreateFromSI(Diameter);
            startPipeEntity.StartPosition = Position;
            startPipeEntity.Projection = pipeProjection;

            if (Name != null)
                startPipeEntity.Name = Name;

            return startPipeEntity;
        }

        [Pure]
        public IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            return new Vector<double>[] { Position, EndPosition };
        }
    }
}