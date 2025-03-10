using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Start.API;
using Start.Entities;

namespace Start
{
    public class StartProject : IDisposable
    {
        private readonly StartAutoServer? _autoServer;
        private readonly StartDocument _document;
        private readonly StartBaseRootDataArray _dataArray;

        public StartProject(StartAutoServer? autoServer, StartDocument document, StartBaseRootDataArray dataArray)
        {
            _autoServer = autoServer;
            _document = document;
            _dataArray = dataArray;
        }

        public static StartProject OpenFromDocument(StartDocument document)
        {
            StartBaseRootDataArray baseRootDataArray = document.GetDataArrayDispatch();
            return new StartProject(null, document, baseRootDataArray);
        }

        public static StartProject OpenProject(string filepath, int mode = 0x4)
        {
            StartAutoServer autoServer = new StartAutoServer();
            StartDocument document = autoServer.LoadStartDocument(mode, filepath);
            StartBaseRootDataArray baseRootDataArray = document.GetDataArrayDispatch();

            return new StartProject(autoServer, document, baseRootDataArray);
        }

        public StartBaseRoot[] GetConnEntities(StartBaseRoot entity, StartElementType type)
        {
            int elementsNumber = _dataArray.GetNumberConns(entity.Id, type, type);
            StartBaseRoot[] startEntities = new StartBaseRoot[elementsNumber];
            for (int i = 0; i < elementsNumber; i++)
            {
                startEntities[i] = entity.GetConnElemOnType(type, i);
            }

            return startEntities;
        }
    
        public StartBaseRoot GetConnEntity(StartBaseRoot entity, StartElementType type)
        {
            return entity.GetConnElemOnType(type, 0);
        }

        public StartBaseRoot[] GetEntities(StartElementType minType, StartElementType maxType)
        {
            int elementsNumber = _dataArray.GetNumberElements(minType, maxType);
            StartBaseRoot[] startEntities = new StartBaseRoot[elementsNumber];
            for (int i = 0; i < elementsNumber; i++)
            {
                startEntities[i] = _dataArray.GetElementDispatch(i, minType, maxType);
            }

            return startEntities;
        }

        public int GetNumberElements(StartElementType minType, StartElementType maxType)
        {
            return _dataArray.GetNumberElements(minType, maxType);
        }

        public string GetDataJson()
        {
            return _dataArray.GetDataJson(StartElementType.ALL, StartElementType.ALL);
        }
        
        public StartDataArrayItem[]? GetDataArrayItems()
        {
            return JsonConvert.DeserializeObject<StartDataArrayItem[]>(GetDataJson());
        }

        public GroupedEntities GroupEntities(StartDataArrayItem[] startDataArrayItems)
        {
            List<StartEntityContainer> twoNodeEntitiesContainers = new List<StartEntityContainer>();
            List<StartEntityContainer> oneNodeEntitiesContainers = new List<StartEntityContainer>();
            List<StartEntityContainer> nodeEntitiesContainers = new List<StartEntityContainer>();

            Dictionary<int, StartAbstractEntity> nodeEntities = new Dictionary<int, StartAbstractEntity>();
            Dictionary<int, StartAbstractEntity> twoNodeEntities = new Dictionary<int, StartAbstractEntity>();
            Dictionary<int, StartAbstractEntity> oneNodeEntities = new Dictionary<int, StartAbstractEntity>();
            Dictionary<int, int[]> twoNodeEntitiesRelations = new Dictionary<int, int[]>();
            Dictionary<int, int> oneNodeEntitiesRelations = new Dictionary<int, int>();

            foreach (StartDataArrayItem startDataArrayItem in startDataArrayItems)
            {
                StartAbstractEntity? startAbstractEntity = StartEntityFactory.CreateEntity(startDataArrayItem);
                if (startAbstractEntity == null) continue;

                switch (startAbstractEntity.Type)
                {
                    case StartElementType.NODE:
                        nodeEntitiesContainers.Add(new StartEntityContainer()
                        {
                            ID = startDataArrayItem.NodeIds[0],
                            Entity = startAbstractEntity,
                        });
                        nodeEntities.Add(startDataArrayItem.NodeIds[0], startAbstractEntity);
                        break;
                    case StartElementType.RIGID_ELEMENT:
                    case StartElementType.PIPE_ELEMENT:
                        twoNodeEntitiesContainers.Add(new StartEntityContainer()
                        {
                            ID = startDataArrayItem.DataArrayIndex,
                            Entity = startAbstractEntity,
                            NodeIDs = startDataArrayItem.NodeIds
                        });
                        twoNodeEntities.Add(startDataArrayItem.DataArrayIndex, startAbstractEntity);
                        twoNodeEntitiesRelations.Add(startDataArrayItem.DataArrayIndex, startDataArrayItem.NodeIds);
                        break;
                    default:
                        oneNodeEntitiesContainers.Add(new StartEntityContainer()
                        {
                            ID = startDataArrayItem.DataArrayIndex,
                            Entity = startAbstractEntity,
                            NodeIDs = startDataArrayItem.NodeIds
                        });
                        oneNodeEntities.Add(startDataArrayItem.DataArrayIndex, startAbstractEntity);
                        oneNodeEntitiesRelations.Add(startDataArrayItem.DataArrayIndex, startDataArrayItem.NodeIds[0]);
                        break;
                }
            }

            return new GroupedEntities()
            {
                twoNodeEntitiesContainers = twoNodeEntitiesContainers.ToArray(),
                oneNodeEntitiesContainers = oneNodeEntitiesContainers.ToArray(),
                nodeEntitiesContainers = nodeEntitiesContainers.ToArray(),
                
                NodeEntities = nodeEntities,
                TwoNodeEntities = twoNodeEntities,
                OneNodeEntities = oneNodeEntities,
                TwoNodeEntitiesRelations = twoNodeEntitiesRelations,
                OneNodeEntitiesRelations = oneNodeEntitiesRelations
            };
        }

        public void Dispose()
        {
            _dataArray.Dispose();
            _document.Dispose();
            _autoServer?.Dispose();
        }
    }
}