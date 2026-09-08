using System;
using IFCConverter.Start.Interfaces;
using IFCConverter.Start.StartProperties;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Exporter.Converters
{
    internal static class StartToIfcPropertyConverter
    {
        public static IfcValue ConvertToIfcValue(object property)
        {
            switch (property)
            {
                case int _:
                    return new IfcInteger(Convert.ToInt32(property));
                case double _:
                    return new IfcReal(Convert.ToDouble(property));
                case string _:
                    return new IfcText(Convert.ToString(property));
                case IStartValueProperty<double> valueProperty:
                    return ConvertToIfcValue(valueProperty);
                case IStartValueProperty<int> intValueProperty:
                    return ConvertToIfcValue(intValueProperty);
                case IStartEnumProperty<Enum> enumProperty:
                    return ConvertToIfcValue(enumProperty);
                default:
                    return new IfcText(property?.ToString() ?? "");
            }
        }

        public static IfcValue ConvertToIfcValue<T>(IStartValueProperty<T> property)
            where T : struct, IComparable<T>
        {
            double doubleProperty = Convert.ToDouble(property.SIProperty);

            switch (property)
            {
                case AngleValueProperty<T> _:
                    return new IfcPositivePlaneAngleMeasure(doubleProperty);
                case AreaValueProperty<T> _:
                    return new IfcAreaMeasure(doubleProperty);
                case ForceValueProperty<T> _:
                    return new IfcForceMeasure(doubleProperty);
                case LengthValueProperty<T> _:
                    return new IfcLengthMeasure(doubleProperty);
                case MassValueProperty<T> _:
                    return new IfcMassMeasure(doubleProperty);
                case MomentValueProperty<T> _:
                    return new IfcLinearMomentMeasure(doubleProperty);
                case PressureValueProperty<T> _:
                    return new IfcPressureMeasure(doubleProperty);
                case TemperatureValueProperty<T> _:
                    return new IfcThermodynamicTemperatureMeasure(doubleProperty);
                default:
                    return new IfcReal(doubleProperty);
            }
        }

        public static IfcValue ConvertToIfcValue<T>(IStartEnumProperty<T> property)
            where T : Enum
        {
            return new IfcText(property.EnumValue.ToString());
        }
    }
}