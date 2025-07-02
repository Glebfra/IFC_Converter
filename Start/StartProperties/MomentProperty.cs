namespace Start.StartProperties
{
    public class MomentProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "N*m";
        public override string StartUnit { get; } = "tf*m";

        private const double FACTOR = 9806.65;

        public MomentProperty(double startProperty) : base(startProperty)
        {
        }
        
        public MomentProperty() {}

        public static MomentProperty Zero => new MomentProperty(0);
        
        public static MomentProperty CreateFromStart(double startProperty)
        {
            MomentProperty forceProperty = new MomentProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static MomentProperty CreateFromSi(double siProperty)
        {
            MomentProperty forceProperty = new MomentProperty();
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