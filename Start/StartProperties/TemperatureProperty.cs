namespace Start.StartProperties
{
    public class TemperatureProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "C";
        public override string StartUnit { get; } = "C";

        public TemperatureProperty(double startProperty) : base(startProperty)
        {
        }

        public static TemperatureProperty Zero => new TemperatureProperty(0);
    }
}