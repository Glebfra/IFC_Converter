namespace Start.StartProperties
{
    public class AngleProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "rad";
        public override string SIUnit { get; } = "rad";

        public AngleProperty(double startProperty) : base(startProperty)
        {
        }

        public static AngleProperty Zero => new AngleProperty(0);
    }
}