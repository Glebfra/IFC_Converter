namespace Start.Interfaces
{
    public interface IStartSegmentDiameterUndefinedEntity : IStartSegmentEntity
    {
        public IStartValueProperty<double> GetDiameter(object? skipEntity = null);
    }
}