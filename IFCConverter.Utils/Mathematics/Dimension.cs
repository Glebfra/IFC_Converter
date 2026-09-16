namespace IFCConverter.Utils.Mathematics
{
    public interface IDimension
    {
        int Dimension { get; }
    }

    public struct Dim2 : IDimension
    {
        public int Dimension => 2;
    }

    public struct Dim3 : IDimension
    {
        public int Dimension => 3;
    }

    public struct Dim4 : IDimension
    {
        public int Dimension => 4;
    }

    public static class Dimension<TDimension>
        where TDimension : struct, IDimension
    {
        public static int Dim
        {
            get
            {
                TDimension dimension = new TDimension();
                return dimension.Dimension;
            }
        }
    }
}