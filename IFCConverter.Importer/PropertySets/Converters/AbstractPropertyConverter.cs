using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.PropertySets.Converters
{
    internal abstract class AbstractPropertyConverter<TSource, TResult> : IPropertyConverter<TSource, TResult>
    {
        [Pure]
        public abstract TResult ReadTyped(TSource source);

        [Pure]
        public object Read(object value)
        {
            return ReadTyped((TSource)value);
        }
    }
}