using System.Runtime.InteropServices;
using IFC_Converter.Start.API;

namespace IFC_Converter.Start.Entities;

public abstract class StartAbstractEntity : IDisposable
{
    public object _entity { get; private set; }
    public StartElementType elementType { get; protected set; }

    public StartAbstractEntity(object entity)
    {
        _entity = entity;
    }

    public void Dispose()
    {
        Marshal.ReleaseComObject(_entity);
    }
}