using System;

namespace Start.StartProperties
{
    public class PressureValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1;
        public override string StartUnit => "t/m2";
        public override string SIUnit => "t/m2";
    }
}