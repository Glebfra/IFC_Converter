using System;
using Newtonsoft.Json;
using Start.API;

namespace Start
{
    public class StartProject : IDisposable
    {
        public readonly StartAutoServer AutoServer;
        public readonly StartDocument Document;
        public readonly StartBaseRootDataArray DataArray;

        public StartProject(StartAutoServer autoServer, StartDocument document, StartBaseRootDataArray dataArray)
        {
            AutoServer = autoServer;
            Document = document;
            DataArray = dataArray;
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

        public string GetDataJson()
        {
            return DataArray.GetDataJson(StartElementType.ALL, StartElementType.ALL);
        }
        
        public StartDataArrayItem[]? GetDataArrayItems()
        {
            return JsonConvert.DeserializeObject<StartDataArrayItem[]>(GetDataJson());
        }

        public void Dispose()
        {
            DataArray.Dispose();
            Document.Dispose();
            AutoServer.Dispose();
        }
    }
}