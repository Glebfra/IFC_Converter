namespace Start.StartProperties
{
    public class NumberProperty : StartAbstractProperty<int>
    {
        public override string SIUnit { get; } = string.Empty;
        public override string StartUnit { get; } = string.Empty;

        public NumberProperty(int startProperty) : base(startProperty)
        {
        }

        public static NumberProperty Zero => new NumberProperty(0);
    }
}