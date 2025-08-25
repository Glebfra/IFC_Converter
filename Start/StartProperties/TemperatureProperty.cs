namespace Start.StartProperties
{
    public class TemperatureProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "°C";
        public override string StartUnit { get; } = "°C";

        public TemperatureProperty(double startProperty) : base(startProperty)
        {
        }
        
        public TemperatureProperty() {}

        public static TemperatureProperty Zero => new TemperatureProperty(0);
        
        public static TemperatureProperty CreateFromStart(double startProperty)
        {
            TemperatureProperty forceProperty = new TemperatureProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static TemperatureProperty CreateFromSi(double siProperty)
        {
            TemperatureProperty forceProperty = new TemperatureProperty();
            forceProperty.StartProperty = forceProperty.ConvertFromSI(siProperty);
            forceProperty.SIProperty = siProperty;
            return forceProperty;
        }
        
        protected override double ConvertFromStart(double startProperty)
        {
            return startProperty;
        }

        protected override double ConvertFromSI(double siProperty)
        {
            return siProperty;
        }
    }
}