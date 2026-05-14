using Start.Interfaces;

namespace Start.API
{
    public readonly struct StartEntityProxy
    {
        public readonly int Index;
        public readonly IStartBaseRoot StartBaseRoot;

        public StartEntityProxy(IStartBaseRoot startBaseRoot, int index)
        {
            StartBaseRoot = startBaseRoot;
            Index = index;
        }
    }
}