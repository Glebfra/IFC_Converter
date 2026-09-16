using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.Interfaces
{
    /// <summary>
    ///     Defines an interface for cone entities in the IFCConverter.Start framework.
    ///     Inherits from the <see cref="IStartEntity" /> interface.
    /// </summary>
    public interface IStartConeEntity : IStartEntity
    {
        /// <summary>
        ///     Gets the collection of points that define the cone.
        ///     Each point is represented as a vector.
        /// </summary>
        IEnumerable<FixedVector<Dim3>> Points { get; }

        /// <summary>
        ///     Gets the collection of diameters corresponding to the points of the cone.
        /// </summary>
        IEnumerable<double> Diameters { get; }
    }
}