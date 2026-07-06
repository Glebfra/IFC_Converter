using System;
using Start.Interfaces;
using Utils;

namespace Start.StartProperties
{
    public class MassValueProperty<T> : StartValueAbstractProperty<T>, IStartValueProperty<T>
        where T : struct, IComparable<T>
    {
        public override string StartUnit => "tf";
        public override string SIUnit => "kg";
        public override double StartToSIFactor => MathExtensions.TfToKg;
    }
}