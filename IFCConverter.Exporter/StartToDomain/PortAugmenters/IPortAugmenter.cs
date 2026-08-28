using IFCConverter.Domain;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.PortAugmenters
{
    internal interface IPortAugmenter
    {
        bool CanAugment(IStartEntity source);
        void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context);
    }
}