using Start.API;

namespace Start.Extensions
{
    public static class StartElementTypeExtensions
    {
        public static StartElementTypeEnum[] TwoNodeElementTypes =
        {
            StartElementTypeEnum.PIPE_ELEMENT,
            StartElementTypeEnum.RIGID_ELEMENT,
            StartElementTypeEnum.CYLINDRICAL_SHELL,
            StartElementTypeEnum.FLEXIBLE_ELEMENT,
            StartElementTypeEnum.CONE_ELEMENT
        };
    }
}