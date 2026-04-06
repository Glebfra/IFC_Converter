using System;
using Start.Interfaces;

namespace Start.StartProperties
{
    public class MassValueProperty<T> : StartValueAbstractProperty<T>, IStartValueProperty<T>
        where T : struct, IComparable<T>
    {
        public override string StartUnit => "tf";
        public override string SIUnit => "tf";
        public override double StartToSIFactor => 1;
    }
}