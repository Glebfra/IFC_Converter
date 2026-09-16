using System.Collections.Generic;
using IFCConverter.Domain;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters
{
    internal interface IEntityMetadataAugmentersRegistry
    {
        IEnumerable<IEntityMetadataAugmenter> ResolveAll(IIfcProduct product, EngineeringModel model, ImportContext context);
    }
}