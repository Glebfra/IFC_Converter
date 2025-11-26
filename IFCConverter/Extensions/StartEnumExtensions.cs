using System;
using Start.API;

namespace IFCConverter.Extensions
{
    internal static class StartEnumExtensions
    {
        public static StartManufacturingTechnologyEnum GetManufacturingTechnology(string rawValue)
        {
            bool isValidTechnology = Enum.TryParse(rawValue, out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);
            return isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS;
        }

        public static StartLeakageCheckEnum GetLeakageCheck(string rawValue)
        {
            bool isValidLeakage = Enum.TryParse(rawValue, out StartLeakageCheckEnum leakageCheckEnum);
            return isValidLeakage ? leakageCheckEnum : StartLeakageCheckEnum.NO;
        }
    }
}