using System.Collections.Generic;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Profiles
{
    public sealed class ProfileLoop
    {
        public IReadOnlyList<FixedVector<Dim2>> Points { get; }
        public bool IsHole { get; }
        
        public ProfileLoop(IReadOnlyList<FixedVector<Dim2>> points, bool isHole)
        {
            Points = points;
            IsHole = isHole;
        }
    }
}