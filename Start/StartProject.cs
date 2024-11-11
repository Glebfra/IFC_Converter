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
        _document = _autoServer.LoadStartDocument(0x2, filepath);
        _dataArray = _document.GetDataArrayDispatch();
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