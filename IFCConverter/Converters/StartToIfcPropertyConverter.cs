using System;
using Start.Interfaces;
using Start.StartProperties;
using Xbim.Ifc4.MeasureResource;

namespace IFCConverter.Converters
{
    public static class StartToIfcPropertyConverter
    {
        public static IfcValue ConvertToIfcValue(object property)
        {
            return property switch
            {
                int => new IfcInteger(Convert.ToInt32(property)),
                double => new IfcReal(Convert.ToDouble(property)),
                string => new IfcText(Convert.ToString(property)),
                IStartValueProperty<double> valueProperty => ConvertToIfcValue(valueProperty),
                IStartValueProperty<int> intValueProperty => ConvertToIfcValue(intValueProperty),
                IStartEnumProperty<Enum> enumProperty => ConvertToIfcValue(enumProperty),
                _ => new IfcText(property?.ToString() ?? "")
            };
        }
        
        public static IfcValue ConvertToIfcValue<T>(IStartValueProperty<T> property) 
            where T : struct, IComparable<T>
        {
            double doubleProperty = Convert.ToDouble(property.SIProperty);
            
            return property switch
            {
                AngleValueProperty<T> => new IfcPositivePlaneAngleMeasure(doubleProperty),
                AreaValueProperty<T> => new IfcAreaMeasure(doubleProperty),
                ForceValueProperty<T> => new IfcForceMeasure(doubleProperty),
                LengthValueProperty<T> => new IfcLengthMeasure(doubleProperty),
                MassValueProperty<T> => new IfcMassMeasure(doubleProperty),
                MomentValueProperty<T> => new IfcLinearMomentMeasure(doubleProperty),
                PressureValueProperty<T> => new IfcPressureMeasure(doubleProperty),
                TemperatureValueProperty<T> => new IfcThermodynamicTemperatureMeasure(doubleProperty),
                _ => new IfcReal(doubleProperty),
            };
        }

        public static IfcValue ConvertToIfcValue<T>(IStartEnumProperty<T> property) 
            where T : Enum
        {
            return new IfcText(property.EnumValue.ToString());
        }
    }
}