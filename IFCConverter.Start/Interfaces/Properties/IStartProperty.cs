namespace IFCConverter.Start.Interfaces
{
    public interface IStartProperty
    {
        object Write();
        object Read(object value);
    }
}