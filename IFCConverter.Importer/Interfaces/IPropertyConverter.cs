using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.Interfaces
{
    public interface IPropertyConverter
    {
        [Pure]
        public object Read(object value);
    }

    public interface IPropertyConverter<in TSource, out TResult> : IPropertyConverter
    {
        [Pure]
        public TResult ReadTyped(TSource source);
    }
}