using MathNet.Numerics.LinearAlgebra;

namespace Start.Interfaces
{
    /// <summary>
    ///     Represents an entity with two nodes, providing properties for positions and directions.
    /// </summary>
    public interface IStartTwoNodeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the starting position of the entity as a vector of doubles.
        /// </summary>
        public Vector<double> StartPosition { get; set; }

        /// <summary>
        ///     Gets or sets the direction vector from the starting position.
        /// </summary>
        public Vector<double> Projection { get; set; }

        /// <summary>
        ///     Gets the ending position of the entity as a vector of doubles.
        /// </summary>
        public Vector<double> EndPosition { get; }

        /// <summary>
        ///     Gets the start node entity of the two node entity.
        /// </summary>
        public IStartNodeEntity StartNode { get; }

        /// <summary>
        ///     Gets the end node entity of the two node entity.
        /// </summary>
        public IStartNodeEntity EndNode { get; }

        /// <summary>
        ///     Gets the start transformation matrix of the two node entity.
        /// </summary>
        public Matrix<double> TransformationMatrix { get; }

        public bool IsStartPosition(Vector<double> position);
    }
}