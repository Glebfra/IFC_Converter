namespace Entities.Entities.Properties
{
    public class ForceProperty : AbstractProperty<double>
    {
        public override string SIUnit { get; } = "N";
        public override string StartUnit { get; } = "tf";

        private const double FACTOR = 9806.65;

        public ForceProperty(double startProperty) : base(startProperty)
        {
        }
        
        public ForceProperty() {}

        public static ForceProperty Zero => new ForceProperty(0);

        public static ForceProperty CreateFromStart(double startProperty)
        {
            ForceProperty forceProperty = new ForceProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static ForceProperty CreateFromSi(double siProperty)
        {
            ForceProperty forceProperty = new ForceProperty();
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