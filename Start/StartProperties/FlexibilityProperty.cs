namespace Start.StartProperties
{
    public class FlexibilityProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "m/kg";
        public override string StartUnit { get; } = "m/t";

        private const double FACTOR = 1e-3;
        
        public FlexibilityProperty(double startProperty) : base(startProperty)
        {
        }

        public static FlexibilityProperty Zero => new FlexibilityProperty(0);

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