using System;
using System.Collections.Generic;
using System.Reflection;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

namespace Start
{
    public class EntityCreator
    {
        private readonly Dictionary<StartElementType, Type> _entityTypeMap;

        public EntityCreator()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            _entityTypeMap = new Dictionary<StartElementType, Type>();
            
            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(StartAbstractEntity))) continue;
                
                object[] attributes = type.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute is not StartEntityTypeAttribute entityAttribute) continue;
                    
                    foreach (StartElementType entityAttributeType in entityAttribute.Types)
                    {
                        _entityTypeMap.Add(entityAttributeType, type);
                    }
                }
            }
        }

        public StartAbstractEntity? CreateEntity(StartDataArrayItem arrayItem)
        {
            if (!_entityTypeMap.ContainsKey(arrayItem.Type)) return null;

            string dataString = arrayItem.Data.ToString();
            StartAbstractEntity abstractEntity = (StartAbstractEntity)JsonConvert.DeserializeObject(dataString, _entityTypeMap[arrayItem.Type])!;
            abstractEntity.Type = arrayItem.Type;
            return abstractEntity;
        }
    }
}