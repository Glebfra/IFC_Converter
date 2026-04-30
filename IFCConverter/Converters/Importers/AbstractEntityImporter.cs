using IFCConverter.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Converters.Importers
{
    internal abstract class AbstractEntityImporter<TSource, TResult> : IEntityImporter<TSource, TResult>
        where TSource : IIfcElement
        where TResult : class
    {
        public abstract TResult ReadTyped(TSource source);
        public object Read(IInstantiableEntity entity) => ReadTyped((TSource)entity);
    }
}