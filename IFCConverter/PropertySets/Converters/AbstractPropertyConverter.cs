using System.Diagnostics.Contracts;
using IFCConverter.Interfaces;

namespace IFCConverter.PropertySets.Converters
{
    internal abstract class AbstractPropertyConverter<TSource, TResult> : IPropertyConverter<TSource, TResult>
    {
        [Pure]
        public abstract TResult ReadTyped(TSource source);

        [Pure]
        public object Read(object value) => ReadTyped((TSource)value)!;
    }
}