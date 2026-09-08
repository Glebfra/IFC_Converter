using System.Diagnostics.Contracts;

namespace IFCConverter.Importer.Interfaces
{
    internal interface IPropertyConverter
    {
        [Pure]
        object Read(object value);
    }

    internal interface IPropertyConverter<in TSource, out TResult> : IPropertyConverter
    {
        [Pure]
        TResult ReadTyped(TSource source);
    }
}