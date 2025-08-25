namespace Start.StartProperties
{
    public sealed class LengthProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "m";
        public override string SIUnit { get; } = "m";

        public LengthProperty(double startProperty) : base(startProperty)
        {
        }
        
        public LengthProperty() {}

        public static LengthProperty Zero => new LengthProperty(0);
        
        public static LengthProperty CreateFromStart(double startProperty)
        {
            LengthProperty forceProperty = new LengthProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static LengthProperty CreateFromSi(double siProperty)
        {
            LengthProperty forceProperty = new LengthProperty();
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