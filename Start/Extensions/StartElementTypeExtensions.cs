using Start.API;

namespace Start.Extensions
{
    public static class StartElementTypeExtensions
    {
        public static StartElementType[] TwoNodeElementTypes = 
        {
            StartElementType.PIPE_ELEMENT,
            StartElementType.RIGID_ELEMENT,
            StartElementType.CYLINDRICAL_SHELL,
            StartElementType.FLEXIBLE_ELEMENT
        };
    }
}