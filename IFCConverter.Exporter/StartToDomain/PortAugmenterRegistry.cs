using IFCConverter.Exporter.StartToDomain.PortAugmenters;
using Start.Interfaces;
using Utils;

namespace IFCConverter.Exporter.StartToDomain
{
    internal sealed class PortAugmenterRegistry : ReflectionRegistry<IPortAugmenter>, IPortAugmenterRegistry
    {
        public PortAugmenterRegistry() : base(typeof(PortAugmenterRegistry).Assembly)
        {
        }

        public IPortAugmenter Resolve(IStartEntity source)
        {
            return Resolve(augmenter => augmenter.CanAugment(source));
        }

        public bool TryResolve(IStartEntity source, out IPortAugmenter result)
        {
            return TryResolve(augmenter => augmenter.CanAugment(source), out result);
        }
    }
}