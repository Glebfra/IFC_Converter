namespace Start.StartProperties
{
    public class ForceProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "N";
        public override string StartUnit { get; } = "tf";

        private const double FACTOR = 9806.65;

        public ForceProperty(double startProperty) : base(startProperty)
        {
        }

        public static ForceProperty Zero => new ForceProperty(0);

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