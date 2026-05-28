using System.Diagnostics.Contracts;

namespace IFCConverter.Exporter.Interfaces
{
    internal interface IIfcElementConverter
    {
        [Pure]
        public object BuildIfc(object start);

        [Pure]
        public object BuildStart(object ifc);
    }
}