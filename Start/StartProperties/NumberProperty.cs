namespace Start.StartProperties
{
    public class NumberProperty : StartAbstractProperty<int>
    {
        public override string SIUnit { get; } = string.Empty;
        public override string StartUnit { get; } = string.Empty;

        public NumberProperty(int startProperty) : base(startProperty)
        {
        }
        
        public NumberProperty() {}

        public static NumberProperty Zero => new NumberProperty(0);
        
        public static NumberProperty CreateFromStart(int startProperty)
        {
            NumberProperty forceProperty = new NumberProperty();
            forceProperty.StartProperty = startProperty;
            forceProperty.SIProperty = forceProperty.ConvertFromStart(startProperty);
            return forceProperty;
        }
        
        public static NumberProperty CreateFromSi(int siProperty)
        {
            NumberProperty forceProperty = new NumberProperty();
            forceProperty.StartProperty = forceProperty.ConvertFromSI(siProperty);
            forceProperty.SIProperty = siProperty;
            return forceProperty;
        }
        
        protected override int ConvertFromStart(int startProperty)
        {
            return startProperty;
        }

        protected override int ConvertFromSI(int siProperty)
        {
            return siProperty;
        }
    }
}