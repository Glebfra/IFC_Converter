using System.Diagnostics.Contracts;

namespace IFCConverter.Interfaces
{
    internal interface IPropertyConverter
    {
        [Pure]
        public object Read(object value);
    }

    internal interface IPropertyConverter<in TSource, out TResult> : IPropertyConverter
    {
        [Pure]
        public TResult ReadTyped(TSource source);
    }
}