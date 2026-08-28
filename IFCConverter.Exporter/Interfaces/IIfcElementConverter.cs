using System.Diagnostics.Contracts;

namespace IFCConverter.Exporter.Interfaces
{
    internal interface IIfcElementConverter
    {
        [Pure]
        object BuildIfc(object start);

        [Pure]
        object BuildStart(object ifc);
    }
}