namespace Start.StartProperties
{
    public class PressureProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "t/m2";
        public override string SIUnit { get; } = "Pa";

        private const double FACTOR = 9806.65;

        public PressureProperty(double startProperty) : base(startProperty)
        {
        }

        public static PressureProperty Zero => new PressureProperty(0);

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