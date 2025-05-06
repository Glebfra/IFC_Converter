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

        public static MomentProperty Zero => new MomentProperty(0);

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