using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Importer.Extensions;
using IFCConverter.Importer.PropertySets.Start;
using IFCConverter.Utils.Collections;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFCConverter.Importer.IfcToDomain.EntityMetadataAugmenters
{
    internal sealed class StartPsetMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanAugment(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            if (!context.TryGetEntityId(product, out Id<EntityId> id))
                return false;

            StartPset startPset = ((IfcProduct)product).GetPropertySets().OfType<StartPset>().FirstOrDefault();
            return startPset != null;
        }

        public void Augment(IIfcProduct product, EngineeringModel model, ImportContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(product));
            StartPset startPset = ((IfcProduct)product).GetPropertySets().OfType<StartPset>().First();

            foreach (KeyValuePair<string, object> pair in startPset.GetNotNullDictionary())
                entity.Metadata.Properties.TryAdd(pair.Key, pair.Value);
        }
    }
}