using System;
using Start.API;

namespace IFCtoSTART.Extensions
{
    internal static class ManufacturingTechnologyExtensions
    {
        public static StartManufacturingTechnologyEnum GetManufacturingTechnology(string rawValue)
        {
            bool isValidTechnology = Enum.TryParse(rawValue, out StartManufacturingTechnologyEnum manufacturingTechnologyEnum);
            return isValidTechnology ? manufacturingTechnologyEnum : StartManufacturingTechnologyEnum.SEAMLESS;
        }
    }
}