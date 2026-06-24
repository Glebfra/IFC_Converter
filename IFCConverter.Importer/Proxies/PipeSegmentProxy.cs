using System.Diagnostics.Contracts;
using IFCConverter.Importer.Attributes;
using IFCConverter.Importer.BoundaryResolvers;
using IFCConverter.Importer.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Importer.Proxies
{
    [ProxyEntity(2, boundaryResolverType: typeof(PipeSegmentBoundaryResolver))]
    internal class PipeSegmentProxy : ISegmentProxy
    {

        public PipeSegmentProxy(double diameter, double length, Vector<double> position, Vector<double> direction)
        {
            Diameter = diameter;
            Length = length;
            Position = position;
            Direction = direction;
        }

        public double Diameter { get; }

        public string? Name { get; set; }
        public double Length { get; }
        public Vector<double> Position { get; }
        public Vector<double> Direction { get; }
        public Vector<double> EndPosition => Position + Direction * Length;

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
    }
}