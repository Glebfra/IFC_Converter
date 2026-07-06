namespace IFCConverter.Importer.Interfaces
{
    internal interface IFittingTopologyEntity : ITopologyEntity
    {
        public ITopologyNodeEntity Node { get; }
    }
}