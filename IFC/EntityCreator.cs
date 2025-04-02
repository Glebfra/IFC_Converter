using System;
using System.Collections.Generic;
using System.Reflection;
using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using Start.API;
using Start.Entities.Abstract;

namespace IFC
{
    public class EntityCreator
    {
        private readonly Dictionary<StartElementType, Type> _entityTypeMap;
        private readonly Dictionary<StartElementType, Type> _vertexEntityTypeMap;

        public EntityCreator()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            _entityTypeMap = new Dictionary<StartElementType, Type>();
            _vertexEntityTypeMap = new Dictionary<StartElementType, Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (!type.IsSubclassOf(typeof(IfcAbstractEntity))) continue;
                
                object[] attributes = type.GetCustomAttributes(false);
                foreach (object attribute in attributes)
                {
                    if (attribute is not IfcEntityTypeAttribute entityAttribute) continue;
                    if (entityAttribute.IsVertex)
                    {
                        foreach (StartElementType entityAttributeType in entityAttribute.Types)
                        {
                            _vertexEntityTypeMap.Add(entityAttributeType, type);
                        }
                    }
                    else
                    {
                        foreach (StartElementType entityAttributeType in entityAttribute.Types)
                        {
                            _entityTypeMap.Add(entityAttributeType, type);
                        }
                    }
                }
            }
        }
        
        public IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities)
        {
            if (!_entityTypeMap.ContainsKey(entity.Type)) 
                return null;
            Type type = _entityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntities);
            return abstractEntity;
        }

        public IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities)
        {
            if (!_entityTypeMap.ContainsKey(entity.Type)) 
                return null;
            Type type = _entityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntity, segmentEntities);
            return abstractEntity;
        }
        
        public IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities)
        {
            if (!_entityTypeMap.ContainsKey(entity.Type)) 
                return null;
            Type type = _entityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntities, segmentEntities);
            return abstractEntity;
        }
        
        public IfcAbstractEntity? CreateVertexEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            if (!_vertexEntityTypeMap.ContainsKey(entity.Type)) 
                return null;
            Type type = _vertexEntityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntity, segmentEntities, numSegments);
            return abstractEntity;
        }
        
        public IfcAbstractEntity? CreateVertexEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity[] segmentEntities, int numSegments)
        {
            if (!_vertexEntityTypeMap.ContainsKey(entity.Type)) 
                return null;
            Type type = _vertexEntityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntities, segmentEntities, numSegments);
            return abstractEntity;
        }
    }
}