using IFC_Converter.Start.API;
using IFC_Converter.Start.Entities;

namespace IFC_Converter.Start;

public class StartProject : IDisposable
{
    private readonly StartAutoServer _autoServer;
    private readonly StartDocument _document;
    private readonly StartBaseRootDataArray _dataArray;

    public StartProject(string filepath)
    {
        _autoServer = new StartAutoServer();
        _document = _autoServer.LoadStartDocument(0x4, filepath);
        _dataArray = _document.GetDataArrayDispatch();
    }

    public T[] GetConnEntities<T>(StartAbstractEntity entity, StartElementType type) where T : StartAbstractEntity
    {
        int elementsNumber = _dataArray.GetNumberConns(entity.Id, type, type);
        T[] entities = new T[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            StartBaseRoot baseRoot = entity.Entity.GetConnElemOnType(type, i);
            entities[i] = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        }

        return entities;
    }

    public T GetConnEntity<T>(StartAbstractEntity entity, StartElementType type) where T : StartAbstractEntity
    {
        StartBaseRoot baseRoot = entity.Entity.GetConnElemOnType(type, 0);
        return (T)Activator.CreateInstance(typeof(T), baseRoot)!;
    }

    public T[] GetEntities<T>(StartElementType type) where T : StartAbstractEntity
    {
        int elementsNumber = _dataArray.GetNumberElements(type, type);
        T[] objs = new T[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            StartBaseRoot baseRoot = _dataArray.GetElementDispatch(i, type, type);
            objs[i] = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        }

        return objs;
    }

    public T[] GetEntities<T>(StartElementType minType, StartElementType maxType) where T : StartAbstractEntity
    {
        int elementsNumber = _dataArray.GetNumberElements(minType, maxType);
        T[] objs = new T[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            StartBaseRoot baseRoot = _dataArray.GetElementDispatch(i, minType, maxType);
            objs[i] = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        }

        return objs;
    }

    public void Dispose()
    {
        _dataArray.Dispose();
        _document.Dispose();
        _autoServer.Dispose();
    }
}