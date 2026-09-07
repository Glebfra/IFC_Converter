using System;

namespace IFCConverter.Start.StartProperties
{
    public class MomentValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1.0;
        public override string StartUnit => "tf*m";
        public override string SIUnit => "tf*m";
    }
}