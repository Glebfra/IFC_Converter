namespace Entities.Entities.Properties
{
    public class FlexibilityProperty : AbstractProperty<double>
    {
        public override string SIUnit { get; } = "m/kg";
        public override string StartUnit { get; } = "m/t";

        private const double FACTOR = 1e-3;
        
        public FlexibilityProperty(double startProperty) : base(startProperty)
        {
        }
        
        public FlexibilityProperty() {}

        public static FlexibilityProperty Zero => new FlexibilityProperty(0);

        public static FlexibilityProperty CreateFromStart(double startProperty)
        {
            FlexibilityProperty flexibilityProperty = new FlexibilityProperty();
            flexibilityProperty.StartProperty = startProperty;
            flexibilityProperty.SIProperty = flexibilityProperty.ConvertFromStart(startProperty);
            return flexibilityProperty;
        }
        
        public static FlexibilityProperty CreateFromSi(double siProperty)
        {
            FlexibilityProperty flexibilityProperty = new FlexibilityProperty();
            flexibilityProperty.StartProperty = flexibilityProperty.ConvertFromSI(siProperty);
            flexibilityProperty.SIProperty = siProperty;
            return flexibilityProperty;
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