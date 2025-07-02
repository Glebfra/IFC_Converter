namespace Start.StartProperties
{
    public sealed class MassProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "tf";
        public override string SIUnit { get; } = "kg";

        private const double FACTOR = 102.04;

        public MassProperty(double startProperty) : base(startProperty)
        {
        }
        
        public MassProperty() {}

        public static MassProperty Zero => new MassProperty(0);
        
        public static MassProperty CreateFromStart(double startProperty)
        {
            MassProperty forceProperty = new MassProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static MassProperty CreateFromSi(double siProperty)
        {
            MassProperty forceProperty = new MassProperty();
            forceProperty.StartProperty = forceProperty.ConvertFromSI(siProperty);
            forceProperty.SIProperty = siProperty;
            return forceProperty;
        }

        protected override double ConvertFromStart(double startProperty)
        {
            return startProperty * FACTOR;
        }

        protected override double ConvertFromSI(double siProperty)
        {
            return siProperty / FACTOR;
        }
    }
}