using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.Interfaces
{
    /// <summary>
    ///     Represents an entity with a single node that has a position in space.
    /// </summary>
    public interface IStartOneNodeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets or sets the position of the node as a vector of doubles.
        /// </summary>
        FixedVector<Dim3> Position { get; set; }

        IStartNodeEntity Node { get; }
    }
}