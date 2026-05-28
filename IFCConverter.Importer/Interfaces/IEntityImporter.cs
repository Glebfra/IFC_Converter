using Xbim.Common;

namespace IFCConverter.Importer.Interfaces
{
    public interface IEntityImporter
    {
        public object Read(IInstantiableEntity entity);
    }
    
    public interface IEntityImporter<in TSource, out TResult> : IEntityImporter
    {
        public TResult ReadTyped(TSource source);
    }
}