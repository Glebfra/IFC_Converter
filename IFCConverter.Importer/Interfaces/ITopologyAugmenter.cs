namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyAugmenter
    {
        public ITopologyModel Augment(ITopologyModel model);
    }
}