namespace Start.StartProperties
{
    public class MassProperty : IStartProperty<double>
    {
        public double StartProperty { get; }
        public double SIProperty { get; }

        public string StartUnit { get; } = "tf";
        public string SIUnit { get; } = "kg";

        private MassProperty(double startProperty, double siProperty)
        {
            StartProperty = startProperty;
            SIProperty = siProperty;
        }

        public static MassProperty CreateFromStart(double startProperty)
        {
            return new MassProperty(startProperty, ConvertFromStart(startProperty));
        }
        
        public static MassProperty CreateFromSI(double siProperty)
        {
            return new MassProperty(ConvertFromSI(siProperty), siProperty);
        }

        private static double ConvertFromStart(double startProperty)
        {
            return startProperty * 1000;
        }

        private static double ConvertFromSI(double siProperty)
        {
            return siProperty / 1000;
        }
    }
}