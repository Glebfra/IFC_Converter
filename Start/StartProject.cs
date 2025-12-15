using System;
using System.Linq;
using Newtonsoft.Json;
using Start.API;
using Start.Entities;
using Start.Entities.Abstract;

namespace Start
{
    public class StartProject : IDisposable
    {
        public readonly StartAutoServer? AutoServer;
        public readonly StartDocument Document;
        public readonly StartBaseRootDataArray DataArray;

        public StartProject(StartAutoServer? autoServer, StartDocument document, StartBaseRootDataArray dataArray)
        {
            AutoServer = autoServer;
            Document = document;
            DataArray = dataArray;
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
            int elementsNumber = DataArray.GetNumberConns(entity.Id, type, type);
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
            int elementsNumber = DataArray.GetNumberElements(minType, maxType);
            StartBaseRoot[] startEntities = new StartBaseRoot[elementsNumber];
            for (int i = 0; i < elementsNumber; i++)
            {
                startEntities[i] = DataArray.GetElementDispatch(i, minType, maxType);
            }

            return startEntities;
        }

        public int GetNumberElements(StartElementType minType, StartElementType maxType)
        {
            return DataArray.GetNumberElements(minType, maxType);
        }

        public StartBaseRoot AddElement(StartElementType type, out int index)
        {
            return new StartBaseRoot(DataArray.AddElement(type, out index));
        }

        public StartBaseRoot AddElementAndNode(StartElementType type, int nSNode, int nENode, out int index)
        {
            return new StartBaseRoot(DataArray.AddElementAndNode(type, nSNode, nENode, out index));
        }
        
        public StartBaseRoot AddElementAndNode(StartElementType type, int nSNode, out int index)
        {
            return new StartBaseRoot(DataArray.AddElementAndNode(type, nSNode, out index));
        }

        public string GetDataJson()
        {
            return DataArray.GetDataJson(StartElementType.ALL, StartElementType.ALL);
        }
        
        public StartDataArrayItem[] GetDataArrayItems()
        {
            StartDataArrayItem[]? allDataArrayItems = JsonConvert.DeserializeObject<StartDataArrayItem[]>(GetDataJson());
            if (allDataArrayItems == null) 
                throw new NullReferenceException($"{nameof(GetDataArrayItems)} Cannot deserialize objects");

            StartDataArrayItem[] dataArrayItems = allDataArrayItems.Select(item => 
            {
                try
                {
                    StartAbstractEntity? entity = StartEntityFactory.CreateEntity(item);
                    if (entity != null)
                    {
                        item.Entity = entity;
                        item.Entity.ID = item.Type == StartElementType.NODE ? item.NodeIds[0] : item.DataArrayIndex;
                        item.Entity.Type = item.Type;
                    }
                    return item;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    return null;
                }
            }).Where(item => item != null && item.Entity != null).ToArray();

            return dataArrayItems;
        }

        public void Dispose()
        {
            DataArray.Dispose();
            Document.Dispose();
            AutoServer?.Dispose();
        }
    }
}