namespace Start.StartProperties
{
    public class LengthProperty : IStartProperty<double>
    {
        public double StartProperty { get; }
        public double SIProperty { get; }

        public string StartUnit { get; } = "m";
        public string SIUnit { get; } = "m";
        
        private LengthProperty(double startProperty, double siProperty)
        {
            StartProperty = startProperty;
            SIProperty = siProperty;
        }

        public static LengthProperty CreateFromStart(double startProperty)
        {
            return new LengthProperty(startProperty, ConvertFromStart(startProperty));
        }
        
        public static LengthProperty CreateFromSI(double siProperty)
        {
            return new LengthProperty(ConvertFromSI(siProperty), siProperty);
        }

        private static double ConvertFromStart(double startProperty)
        {
            return startProperty;
        }

        private static double ConvertFromSI(double siProperty)
        {
            return siProperty;
        }
    }
}