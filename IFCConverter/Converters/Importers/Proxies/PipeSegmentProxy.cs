using System.Collections.Generic;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal class PipeSegmentProxy : IBoundaryEntityProxy
    {
        public readonly double Diameter;
        public readonly double Length;
        public readonly Vector<double> Position;
        public readonly Vector<double> Direction;

        public string? Name { get; set; }
        
        public PipeSegmentProxy(double diameter, double length, Vector<double> position, Vector<double> direction)
        {
            Diameter = diameter;
            Length = length;
            Position = position;
            Direction = direction;
        }

        public IStartEntity ToStartEntity()
        {
            Vector<double> pipeProjection = Direction * Length;

            StartPipeEntity startPipeEntity = new StartPipeEntity();
            startPipeEntity.Diameter.CreateFromSI(Diameter);
            startPipeEntity.StartPosition = Position;
            startPipeEntity.Projection = pipeProjection;

            if (Name != null)
                startPipeEntity.Name = Name;
            
            startPipeEntity.ConnectedEntities.Add(new StartNodeEntity { Position = Position });
            startPipeEntity.ConnectedEntities.Add(new StartNodeEntity { Position = Position + pipeProjection });

            return startPipeEntity;
        }

        public IEnumerable<Vector<double>> GetBoundaryPoints()
        {
            return new Vector<double>[] { Position, Position + Direction * Length };
        }
    }
}