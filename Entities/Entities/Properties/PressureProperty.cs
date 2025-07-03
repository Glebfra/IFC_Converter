namespace Entities.Entities.Properties
{
    public class PressureProperty : AbstractProperty<double>
    {
        public override string StartUnit { get; } = "t/m2";
        public override string SIUnit { get; } = "Pa";

        private const double FACTOR = 9806.65;

        public PressureProperty(double startProperty) : base(startProperty)
        {
        }
        
        public PressureProperty() {}

        public static PressureProperty Zero => new PressureProperty(0);
        
        public static PressureProperty CreateFromStart(double startProperty)
        {
            PressureProperty forceProperty = new PressureProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static PressureProperty CreateFromSi(double siProperty)
        {
            PressureProperty forceProperty = new PressureProperty();
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