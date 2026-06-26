namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyModelAugmenter
    {
        public ITopologyModel Augment(ITopologyModel model);
    }
}