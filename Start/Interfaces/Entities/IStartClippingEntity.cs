namespace Start.Interfaces
{
    /// <summary>
    ///     Defines an interface for entities that can perform clipping operations on clippable entities
    ///     in the Start framework.
    /// </summary>
    public interface IStartClippingEntity
    {
        /// <summary>
        ///     Clips the specified clippable entity.
        /// </summary>
        /// <param name="clippable">The clippable entity to be clipped.</param>
        public void ClipEntity(IStartClippableEntity clippable);
    }
}