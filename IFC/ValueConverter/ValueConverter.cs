namespace IFC.ValueConverter
{
    public static class ValueConverter
    {
        public const double g = 9.80665;

        public static double T_m2ToPa(double value)
        {
            return value * 1000 * g;
        }

        public static double T_mToKg_m(double value)
        {
            return value / 1000;
        }

        public static double TfToN(double value)
        {
            return value * 1000 * g;
        }
    
        public static double TfToKg(double value)
        {
            return value * 1000;
        }
    }
}