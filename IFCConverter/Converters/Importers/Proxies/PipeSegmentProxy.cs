using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Segments;
using Start.Interfaces;

namespace IFCConverter.Converters.Importers.Proxies
{
    internal class PipeSegmentProxy : IEntityProxy
    {
        public readonly string Name;
        public readonly double Diameter;
        public readonly double Length;
        public readonly Vector<double> Position;
        public readonly Vector<double> Direction;

        public PipeSegmentProxy(double diameter, double length, Vector<double> position, Vector<double> direction, string name)
        {
            Diameter = diameter;
            Length = length;
            Position = position;
            Direction = direction;
            Name = name;
        }

        public IStartEntity ToStartEntity()
        {
            StartPipeEntity startPipeEntity = new StartPipeEntity();
            startPipeEntity.Diameter.CreateFromSI(Diameter);
            startPipeEntity.StartPosition = Position;

            Vector<double> projection = Direction * Length;
            startPipeEntity.ProjectionAlongOXAxis.CreateFromSI(projection[0]);
            startPipeEntity.ProjectionAlongOYAxis.CreateFromSI(projection[1]);
            startPipeEntity.ProjectionAlongOZAxis.CreateFromSI(projection[2]);

            return startPipeEntity;
        }
    }
}