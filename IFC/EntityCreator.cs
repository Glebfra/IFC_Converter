using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IFC.Entities.Abstract;
using IFC.Entities.Fittings;
using Start.API;
using Start.Entities;

namespace IFC
{
    public class EntityCreator
    {
        private static string[] _namespaces =
        {
            "IFC.Entities.Fittings.CAD",
            "IFC.Entities.Fittings.Vertex",
            "IFC.Entities.Fittings",
            "IFC.Entities.Segments",
        };
        private readonly Dictionary<StartElementType, Type> _entityTypeMap;
        private readonly Dictionary<StartElementType, Type> _vertexEntityTypeMap;

        public EntityCreator()
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            _entityTypeMap = new Dictionary<StartElementType, Type>();
            _vertexEntityTypeMap = new Dictionary<StartElementType, Type>();

            foreach (Type type in assembly.GetTypes())
            {
                if (!_namespaces.Contains(type.Namespace) || !type.IsSubclassOf(typeof(StartAbstractEntity))) continue;
                
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

        public IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity segmentEntity)
        {
            if (!_entityTypeMap.ContainsKey(entity.Type)) return null;
            Type type = _entityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntity, segmentEntity)!;
            return abstractEntity;
        }
        
        public IfcAbstractEntity? CreateEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity segmentEntity)
        {
            if (!_entityTypeMap.ContainsKey(entity.Type)) return null;
            Type type = _entityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntities, segmentEntity)!;
            return abstractEntity;
        }
        
        public IfcAbstractEntity? CreateVertexEntity(StartAbstractEntity entity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity segmentEntity, int numSegments)
        {
            if (!_vertexEntityTypeMap.ContainsKey(entity.Type)) return null;
            Type type = _vertexEntityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntity, segmentEntity, numSegments)!;
            return abstractEntity;
        }
        
        public IfcAbstractEntity? CreateVertexEntity(StartAbstractEntity entity, IfcNodeEntity[] nodeEntities, IfcAbstractSegmentEntity segmentEntity, int numSegments)
        {
            if (!_vertexEntityTypeMap.ContainsKey(entity.Type)) return null;
            Type type = _vertexEntityTypeMap[entity.Type];
            
            IfcAbstractEntity abstractEntity = (IfcAbstractEntity)Activator.CreateInstance(type, entity, nodeEntities, segmentEntity, numSegments)!;
            return abstractEntity;
        }
    }
}