using System;

namespace Start.StartProperties
{
    public class AngleValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1;
        public override string StartUnit => "rad";
        public override string SIUnit => "rad";
    }
}