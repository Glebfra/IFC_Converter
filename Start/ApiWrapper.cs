using IFC_Converter.Start.API;

namespace IFC_Converter.Start;

public class ApiWrapper : IDisposable
{
    private StartAutoServer _startAutoServer;

    public ApiWrapper()
    {
        _startAutoServer = new StartAutoServer();
    }

    public void Dispose()
    {
        _startAutoServer.Dispose();
    }
}