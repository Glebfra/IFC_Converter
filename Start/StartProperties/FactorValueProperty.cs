using System;

namespace Start.StartProperties
{
    public class FactorValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1;
        public override string StartUnit => string.Empty;
        public override string SIUnit => string.Empty;
    }
}