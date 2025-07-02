namespace Start.StartProperties
{
    public class FactorProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = string.Empty;
        public override string StartUnit { get; } = string.Empty;

        protected FactorProperty(double startProperty) : base(startProperty)
        {
        }
        
        protected FactorProperty() {}

        public static FactorProperty Zero => new FactorProperty(0);

        public static FactorProperty CreateFromStart(double startProperty)
        {
            FactorProperty factorProperty = new FactorProperty();
            factorProperty.StartProperty = startProperty;
            factorProperty.SIProperty = factorProperty.ConvertFromStart(startProperty);
            return factorProperty;
        }
        
        public static FactorProperty CreateFromSi(double siProperty)
        {
            FactorProperty factorProperty = new FactorProperty();
            factorProperty.StartProperty = factorProperty.ConvertFromSI(siProperty);
            factorProperty.SIProperty = siProperty;
            return factorProperty;
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