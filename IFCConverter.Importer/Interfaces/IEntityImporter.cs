using Xbim.Common;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IEntityImporter
    {
        object Read(IInstantiableEntity entity);
    }

    internal interface IEntityImporter<in TSource, out TResult> : IEntityImporter
    {
        TResult ReadTyped(TSource source);
    }
}