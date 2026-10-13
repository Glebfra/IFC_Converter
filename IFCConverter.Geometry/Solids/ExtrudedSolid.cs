using IFCConverter.Geometry.Profiles;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Geometry.Solids
{
    public readonly struct ExtrudedSolid : IGeometry
    {
        public Profile2D Profile { get; }
        public FixedVector<Dim3> Direction { get; }
        public double Depth { get; }
        
        public ExtrudedSolid(Profile2D profile, FixedVector<Dim3> direction, double depth)
        {
            Profile = profile;
            Direction = direction;
            Depth = depth;
        }
    }
}