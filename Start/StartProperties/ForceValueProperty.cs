using System;

namespace Start.StartProperties
{
    public class ForceValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1.0;
        public override string StartUnit => "tf";
        public override string SIUnit => "tf";
    }
}