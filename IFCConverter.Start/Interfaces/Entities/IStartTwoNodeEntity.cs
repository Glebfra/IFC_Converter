using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.Interfaces
{
    /// <summary>
    ///     Represents an entity with two nodes, providing properties for positions and directions.
    /// </summary>
    public interface IStartTwoNodeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the starting position of the entity as a vector of doubles.
        /// </summary>
        FixedVector<Dim3> StartPosition { get; set; }

        /// <summary>
        ///     Gets or sets the direction vector from the starting position.
        /// </summary>
        FixedVector<Dim3> Projection { get; set; }

        /// <summary>
        ///     Gets the ending position of the entity as a vector of doubles.
        /// </summary>
        FixedVector<Dim3> EndPosition { get; }

        /// <summary>
        ///     Gets the start node entity of the two node entity.
        /// </summary>
        IStartNodeEntity StartNode { get; }

        /// <summary>
        ///     Gets the end node entity of the two node entity.
        /// </summary>
        IStartNodeEntity EndNode { get; }

        /// <summary>
        ///     Gets the start transformation matrix of the two node entity.
        /// </summary>
        FixedMatrix<Dim4> TransformationMatrix { get; }

        bool IsStartPosition(FixedVector<Dim3> position);
    }
}