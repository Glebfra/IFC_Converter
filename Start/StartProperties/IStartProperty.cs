namespace Start.StartProperties
{
    public interface IStartProperty<out T>
    {
        public T StartProperty { get; }
        public T SIProperty { get; }
        
        public string StartUnit { get; }
        public string SIUnit { get; }
    }
}