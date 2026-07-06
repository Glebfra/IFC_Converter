using System;

namespace Start.StartProperties
{
    public class AreaValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1.0;
        public override string StartUnit => "m2";
        public override string SIUnit => "m2";
    }
}