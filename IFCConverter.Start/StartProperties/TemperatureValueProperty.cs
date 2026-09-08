using System;

namespace IFCConverter.Start.StartProperties
{
    public class TemperatureValueProperty<T> : StartValueAbstractProperty<T>
        where T : struct, IComparable<T>
    {
        public override double StartToSIFactor => 1;
        public override string StartUnit => "°C";
        public override string SIUnit => "°C";
    }
}