using Xbim.Common;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IEntityImporter
    {
        public object Read(IInstantiableEntity entity);
    }

    internal interface IEntityImporter<in TSource, out TResult> : IEntityImporter
    {
        public TResult ReadTyped(TSource source);
    }
}