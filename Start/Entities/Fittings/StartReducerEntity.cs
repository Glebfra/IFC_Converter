using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start.Entities.Fittings
{
    public class StartReducerEntity : StartAbstractFittingEntity
    {
        [JsonProperty(StartPropertyName.ConicalPartLength)]
        public double LengthOfConicalPart { get; set; }
        
        [JsonProperty(StartPropertyName.Diameter)] 
        public double MaxDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.MinDiameter)]
        public double MinDiameter { get; set; }
        
        [JsonProperty(StartPropertyName.WallThickness)] 
        public double ThicknessAtMaxDiameterPoint { get; set; }
        
        [JsonProperty(StartPropertyName.MillTolerance)]
        public double MillToleranceAtDMax { get; set; }

        [JsonProperty(StartPropertyName.ManufacturingTechnology)]
        public StartManufacturingTechnologyEnum ManufacturingTechnologyEnum { get; set; }
        
        [JsonProperty(StartPropertyName.AngleBetweenEccentricityVectorAndZmAxis)]
        public double AngleBetweenEccentricityVectorAndZmAxis { get; set; }
        
        [JsonProperty(StartPropertyName.PipeName)]
        public string Name { get; set; }
        
        [JsonProperty(StartPropertyName.MillToleranceAtDMin)]
        public double MillToleranceAtDMin { get; set; }
        
        [JsonProperty(StartPropertyName.ReducerMillTolerance)]
        public double MillTolerance { get; set; }

        public override Dictionary<string, string> GetData()
        {
            Dictionary<string, string> dictionary = base.GetData();
            dictionary.Add("Name", Name);
            dictionary.Add("Manufacturing Technology", ManufacturingTechnologyEnum.ToString());
            dictionary.Add("Mill Tolerance At D Max", MillToleranceAtDMax.ToString("F5"));
            dictionary.Add("Mill Tolerance At D Min", MillToleranceAtDMin.ToString("F5"));
            dictionary.Add("Mill Tolerance", MillTolerance.ToString("F5"));
            dictionary.Add("Length Of Conical Part", LengthOfConicalPart.ToString("F5"));
            dictionary.Add("Max Diameter", MaxDiameter.ToString("F5"));
            dictionary.Add("Min Diameter", MinDiameter.ToString("F5"));
            dictionary.Add("Thickness At Max Diameter Point", ThicknessAtMaxDiameterPoint.ToString("F5"));
            dictionary.Add("Angle Between Eccentricity Vector And Zm Axis", AngleBetweenEccentricityVectorAndZmAxis.ToString("F5"));

            return dictionary;
        }
    }
}