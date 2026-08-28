using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.StartProperties
{
    public class ForceValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => MathExtensions.TfToN;
        public override string StartUnit => "tf";
        public override string SIUnit => "N";
    }
}