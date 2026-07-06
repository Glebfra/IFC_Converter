namespace Start.Interfaces
{
    /// <summary>
    ///     Represents an entity that is defined by a segment with two nodes.
    /// </summary>
    public interface IStartSegmentEntity : IStartEntity, IStartTwoNodeEntity
    {
        /// <summary>
        ///     Gets or sets the length of the segment.
        /// </summary>
        public double Length { get; }

        public IStartValueProperty<double> Diameter { get; set; }
    }
}