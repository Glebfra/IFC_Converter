namespace Start.StartProperties
{
    public class AreaProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "m2";
        public override string StartUnit { get; } = "m2";

        public AreaProperty(double startProperty) : base(startProperty)
        {
        }

        public static AreaProperty Zero => new AreaProperty(0);
    }
}