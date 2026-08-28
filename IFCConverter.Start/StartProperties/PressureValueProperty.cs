using System;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.StartProperties
{
    public class PressureValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => MathExtensions.T_m2ToPa;
        public override string StartUnit => "t/m2";
        public override string SIUnit => "Pa";
    }
}