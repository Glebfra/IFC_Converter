namespace IFCConverter.Tools
{
    internal class IndexedResult<T>
    {
        public T Object;
        public int Index;
        
        public IndexedResult(T @object, int index)
        {
            Object = @object;
            Index = index;
        }
    }
}