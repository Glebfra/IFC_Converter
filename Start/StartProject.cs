using Start.API;
using Start.Entities.Abstract;

namespace Start;

public class StartProject : IDisposable
{
    private readonly StartAutoServer _autoServer;
    private readonly StartDocument _document;
    private readonly StartBaseRootDataArray _dataArray;

    private List<StartAbstractEntity> _abstractEntities;

    public StartProject(StartAutoServer autoServer, StartDocument document, StartBaseRootDataArray dataArray)
    {
        _autoServer = autoServer;
        _document = document;
        _dataArray = dataArray;

        _abstractEntities = new List<StartAbstractEntity>();
    }

    public static StartProject OpenProject(string filepath, int mode = 0x4)
    {
        StartAutoServer autoServer = new StartAutoServer();
        StartDocument document = autoServer.LoadStartDocument(mode, filepath);
        StartBaseRootDataArray baseRootDataArray = document.GetDataArrayDispatch();

        return new StartProject(autoServer, document, baseRootDataArray);
    }

    public T[] GetConnEntities<T>(StartAbstractEntity entity, StartElementType type) where T : StartAbstractEntity
    {
        int elementsNumber = _dataArray.GetNumberConns(entity.Id, type, type);
        T[] objects = new T[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            StartBaseRoot baseRoot = entity.Entity.GetConnElemOnType(type, i);
            objects[i] = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        }
        _abstractEntities.AddRange(objects);

        return objects;
    }

    public T GetConnEntity<T>(StartAbstractEntity entity, StartElementType type) where T : StartAbstractEntity
    {
        StartBaseRoot baseRoot = entity.Entity.GetConnElemOnType(type, 0);
        T @object = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        _abstractEntities.Add(@object);
        
        return @object;
    }

    public T[] GetEntities<T>(StartElementType type) where T : StartAbstractEntity
    {
        int elementsNumber = _dataArray.GetNumberElements(type, type);
        T[] objects = new T[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            StartBaseRoot baseRoot = _dataArray.GetElementDispatch(i, type, type);
            objects[i] = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        }

        return objects;
    }

    public T[] GetEntities<T>(StartElementType minType, StartElementType maxType) where T : StartAbstractEntity
    {
        int elementsNumber = _dataArray.GetNumberElements(minType, maxType);
        T[] objects = new T[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            StartBaseRoot baseRoot = _dataArray.GetElementDispatch(i, minType, maxType);
            objects[i] = (T)Activator.CreateInstance(typeof(T), baseRoot)!;
        }

        return objects;
    }

    public int GetNumberElements(StartElementType minType, StartElementType maxType)
    {
        return _dataArray.GetNumberElements(minType, maxType);
    }

    public void Dispose()
    {
        foreach (StartAbstractEntity entity in _abstractEntities)
        {
            entity.Dispose();
        }
        
        _dataArray.Dispose();
        _document.Dispose();
        _autoServer.Dispose();
    }
}