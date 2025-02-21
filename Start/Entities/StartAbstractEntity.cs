using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Start.API;

namespace Start.Entities
{
    public abstract class StartAbstractEntity
    {
        public int Id { get; protected set; }
        
        public abstract Dictionary<string, string> GetData();

        public static T CreateFromJson<T>(string json, int id = 0)
            where T : StartAbstractEntity
        {
            T abstractArmatureEntity = JsonConvert.DeserializeObject<T>(json) ?? throw new Exception("Cannot deserialize start entity");
            abstractArmatureEntity.Id = id;
            return abstractArmatureEntity;
        }

        public static T CreateFromStartObject<T>(StartBaseRoot startBaseRoot)
            where T : StartAbstractEntity
        {
            return CreateFromJson<T>(startBaseRoot.GetDataJson(), startBaseRoot.Id);
        }
    }
}