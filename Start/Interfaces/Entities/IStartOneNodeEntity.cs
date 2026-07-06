using MathNet.Numerics.LinearAlgebra;

namespace Start.Interfaces
{
    /// <summary>
    ///     Represents an entity with a single node that has a position in space.
    /// </summary>
    public interface IStartOneNodeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the position of the node as a vector of doubles.
        /// </summary>
        public Vector<double> Position { get; set; }

        public IStartNodeEntity Node { get; }
    }
}