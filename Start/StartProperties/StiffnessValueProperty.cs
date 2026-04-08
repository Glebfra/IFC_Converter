using System;
using Utils;

namespace Start.StartProperties
{
    public class StiffnessValueProperty<T> : StartValueAbstractProperty<T> 
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => MathExtensions.MmToM;
        public override string StartUnit => "mm/kg";
        public override string SIUnit => "m/kg";
    }
}