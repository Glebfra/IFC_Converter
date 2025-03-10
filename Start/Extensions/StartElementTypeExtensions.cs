using Start.API;

namespace Start.Extensions
{
    public static class StartElementTypeExtensions
    {
        public static StartElementType[] TwoNodeElementTypes = new[]
        {
            StartElementType.PIPE_ELEMENT,
            StartElementType.RIGID_ELEMENT
        };

        public static StartElementType[] OneNodeElementTypes = new[]
        {
            StartElementType.PIPE_ELEMENT,
            StartElementType.RIGID_ELEMENT
        };
    }
}