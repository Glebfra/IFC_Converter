using IFCConverter.Interfaces;

namespace IFCConverter.PropertySets.Converters
{
    internal abstract class AbstractPropertyConverter<TSource, TResult> : IPropertyConverter<TSource, TResult>
    {
        public abstract TResult ReadTyped(TSource source);

        public object Read(object value) => ReadTyped((TSource)value)!;
    }
}