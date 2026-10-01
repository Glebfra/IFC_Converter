using System.Collections.Generic;

namespace IFCConverter.Geometry.Profiles
{
    public sealed class Profile2D : IGeometry
    {
        public IReadOnlyList<ProfileLoop> Loops { get; }
        
        public Profile2D(IReadOnlyList<ProfileLoop> loops)
        {
            Loops = loops;
        }
    }
}