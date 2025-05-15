namespace Start.StartProperties
{
    public class FactorProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = string.Empty;
        public override string StartUnit { get; } = string.Empty;

        public FactorProperty(double startProperty) : base(startProperty)
        {
        }

        public static FactorProperty Zero => new FactorProperty(0);
    }
}