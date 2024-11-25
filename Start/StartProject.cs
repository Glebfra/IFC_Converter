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

    public StartPipeEntity[] GetConnPipes(StartAbstractEntity startAbstractEntity)
    {
        int elementsNumber = _dataArray.GetNumberConns(startAbstractEntity.Id, StartElementType.PIPE_ELEMENT,
            StartElementType.PIPE_ELEMENT);
        StartPipeEntity[] pipes = new StartPipeEntity[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            pipes[i] = new StartPipeEntity(
                startAbstractEntity.entity.GetConnElemOnType(StartElementType.PIPE_ELEMENT, i));
        }

        return pipes;
    }

    public StartNodeEntity GetConnNode(StartAbstractEntity startAbstractEntity)
    {
        StartNodeEntity node =
            new StartNodeEntity(startAbstractEntity.entity.GetConnElemOnType(StartElementType.NODE, 0));
        return node;
    }

    public StartWeldedTeeEntity[] GetWeldingTees()
    {
        int elementsNumber = _dataArray.GetNumberElements(StartElementType.WELDED_TEE, StartElementType.WELDED_TEE);
        StartWeldedTeeEntity[] tees = new StartWeldedTeeEntity[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            tees[i] = new StartWeldedTeeEntity(_dataArray.GetElementDispatch(i, StartElementType.WELDED_TEE,
                StartElementType.WELDED_TEE));
        }

        return tees;
    }

    public StartNodeEntity[] GetNodes()
    {
        int elementsNumber = _dataArray.GetNumberElements(StartElementType.NODE, StartElementType.NODE);
        StartNodeEntity[] nodes = new StartNodeEntity[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            nodes[i] = new StartNodeEntity(_dataArray.GetElementDispatch(i, StartElementType.NODE,
                StartElementType.NODE));
        }

        return nodes;
    }

    public StartPipeEntity[] GetPipes()
    {
        int elementsNumber = _dataArray.GetNumberElements(StartElementType.PIPE_ELEMENT, StartElementType.PIPE_ELEMENT);
        StartPipeEntity[] pipes = new StartPipeEntity[elementsNumber];
        for (int i = 0; i < elementsNumber; i++)
        {
            pipes[i] = new StartPipeEntity(_dataArray.GetElementDispatch(i, StartElementType.PIPE_ELEMENT,
                StartElementType.PIPE_ELEMENT));
        }

        return pipes;
    }

    public void Dispose()
    {
        _dataArray.Dispose();
        _document.Dispose();
        _autoServer.Dispose();
    }
}