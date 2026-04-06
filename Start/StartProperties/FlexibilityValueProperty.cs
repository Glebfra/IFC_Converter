using System;

namespace Start.StartProperties
{
    public class FlexibilityValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1.0;

        public override string StartUnit => "m/t";
        public override string SIUnit => "m/t";
    }
}