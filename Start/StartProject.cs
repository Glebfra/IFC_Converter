using System;
using System.Linq;
using Newtonsoft.Json;
using Start.API;
using Start.Entities.Abstract;

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
        
        public StartDataArrayItem[] GetDataArrayItems()
        {
            EntityCreator entityCreator = new EntityCreator();
            
            StartDataArrayItem[]? allDataArrayItems = JsonConvert.DeserializeObject<StartDataArrayItem[]>(GetDataJson());
            if (allDataArrayItems == null) throw new NullReferenceException("Cannot deserialize objects");
            StartDataArrayItem[] dataArrayItems = allDataArrayItems.Select(item =>
            {
                StartAbstractEntity? entity = entityCreator.CreateEntity(item);
                if (entity != null)
                {
                    item.Entity = entity;
                    item.Entity.ID = item.Type == StartElementType.NODE ? item.NodeIds[0] : item.DataArrayIndex;
                    item.Entity.Type = item.Type;
                }
                return item;
            }).Where(item => item.Entity != null).ToArray();

            return dataArrayItems;
        }

        public void Dispose()
        {
            _dataArray.Dispose();
            _document.Dispose();
            _autoServer?.Dispose();
        }
    }
}