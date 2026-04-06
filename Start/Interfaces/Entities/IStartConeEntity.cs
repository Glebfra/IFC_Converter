using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;

namespace Start.Interfaces
{
    /// <summary>
    ///     Defines an interface for cone entities in the Start framework.
    ///     Inherits from the <see cref="IStartEntity" /> interface.
    /// </summary>
    public interface IStartConeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets the collection of points that define the cone.
        ///     Each point is represented as a vector.
        /// </summary>
        public IEnumerable<Vector<double>> Points { get; }

        /// <summary>
        ///     Gets the collection of diameters corresponding to the points of the cone.
        /// </summary>
        public IEnumerable<double> Diameters { get; }
    }
}