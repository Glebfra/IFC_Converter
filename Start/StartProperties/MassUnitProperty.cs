namespace Start.StartProperties
{
    public class MassUnitProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "t/m";
        public override string SIUnit { get; } = "kg/m";

        private const double FACTOR = 1000;

        public MassUnitProperty(double startProperty) : base(startProperty)
        {
        }
        
        public MassUnitProperty() {}

        public static MassUnitProperty Zero => new MassUnitProperty(0);
        
        public static MassUnitProperty CreateFromStart(double startProperty)
        {
            MassUnitProperty forceProperty = new MassUnitProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static MassUnitProperty CreateFromSi(double siProperty)
        {
            MassUnitProperty forceProperty = new MassUnitProperty();
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