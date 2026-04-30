namespace IFCConverter.Interfaces
{
    internal interface IPropertyConverter
    {
        public object Read(object value);
    }

    internal interface IPropertyConverter<in TSource, out TResult> : IPropertyConverter
    {
        public TResult ReadTyped(TSource source);
    }
}