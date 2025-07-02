namespace Start.StartProperties
{
    public class AreaProperty : StartAbstractProperty<double>
    {
        public override string SIUnit { get; } = "m2";
        public override string StartUnit { get; } = "m2";

        protected AreaProperty(double startProperty) : base(startProperty)
        {
        }
        
        protected AreaProperty() {}

        public static AreaProperty Zero => new AreaProperty(0);

        public static AreaProperty CreateFromStart(double startProperty)
        {
            AreaProperty areaProperty = new AreaProperty();
            areaProperty.StartProperty = startProperty;
            areaProperty.SIProperty = areaProperty.ConvertFromStart(startProperty);
            return areaProperty;
        }
        
        public static AreaProperty CreateFromSi(double siProperty)
        {
            AreaProperty areaProperty = new AreaProperty();
            areaProperty.StartProperty = areaProperty.ConvertFromSI(siProperty);
            areaProperty.SIProperty = siProperty;
            return areaProperty;
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