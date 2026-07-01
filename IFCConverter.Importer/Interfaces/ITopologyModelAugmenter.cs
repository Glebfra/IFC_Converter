namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyModelAugmenter
    {
        public void Augment(ref ITopologyModel model);
    }
}