namespace Start.StartProperties
{
    public sealed class MassProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "tf";
        public override string SIUnit { get; } = "kg";

        private const double FACTOR = 1000;

        public MassProperty(double startProperty) : base(startProperty)
        {
        }

        public static MassProperty Zero => new MassProperty(0);

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