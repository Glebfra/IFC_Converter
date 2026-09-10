using System.Collections.Generic;

namespace IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers
{
    internal interface IEntityConnectionResolversRegistry
    {
        IEnumerable<IEntityConnectionResolver> ResolveAll();
    }
}