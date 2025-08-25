namespace Start.StartProperties
{
    public class AngleProperty : StartAbstractProperty<double>
    {
        public override string StartUnit { get; } = "rad";
        public override string SIUnit { get; } = "rad";

        public AngleProperty(double startProperty) : base(startProperty)
        {
        }
        
        protected AngleProperty() {}

        public static AngleProperty Zero => new AngleProperty(0);

        public static AngleProperty CreateFromStart(double startProperty)
        {
            AngleProperty angleProperty = new AngleProperty();
            angleProperty.StartProperty = startProperty;
            angleProperty.SIProperty = angleProperty.ConvertFromStart(startProperty);
            return angleProperty;
        }

        public static AngleProperty CreateFromSi(double siProperty)
        {
            AngleProperty angleProperty = new AngleProperty();
            angleProperty.StartProperty = angleProperty.ConvertFromSI(siProperty);
            angleProperty.SIProperty = siProperty;
            return angleProperty;
        }
        
        protected override double ConvertFromStart(double startProperty)
        {
            return startProperty;
        }

        protected override double ConvertFromSI(double siProperty)
        {
            return siProperty;
        }
    }
}