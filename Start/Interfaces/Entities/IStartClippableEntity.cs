using MathNet.Numerics.LinearAlgebra;

namespace Start.Interfaces
{
    /// <summary>
    ///     Defines an interface for entities that can be clipped in the Start framework.
    ///     Inherits from the <see cref="IStartEntity" /> interface.
    /// </summary>
    public interface IStartClippableEntity : IStartEntity
    {
        /// <summary>
        ///     Clips the entity based on the specified position and length.
        /// </summary>
        /// <param name="position">The position vector where the clipping starts.</param>
        /// <param name="length">The length of the clipping operation.</param>
        public void Clip(Vector<double> position, double length);
    }
}