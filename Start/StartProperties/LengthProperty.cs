namespace Start.StartProperties
{
    public sealed class LengthProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "m";
        public override string SIUnit { get; } = "m";
        
        public LengthProperty(double startProperty) : base(startProperty)
        {
        }

        public static LengthProperty Zero => new LengthProperty(0);
    }
}