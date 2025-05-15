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

        public static MassUnitProperty Zero => new MassUnitProperty(0);

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