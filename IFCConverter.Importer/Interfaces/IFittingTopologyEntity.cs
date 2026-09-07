namespace IFCConverter.Importer.Interfaces
{
    internal interface IFittingTopologyEntity : ITopologyEntity
    {
        ITopologyNodeEntity Node { get; }
    }
}