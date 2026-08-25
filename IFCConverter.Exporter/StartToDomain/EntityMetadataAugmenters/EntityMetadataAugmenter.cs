using System;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using Start.API;
using Start.Extensions;
using Start.Interfaces;
using Xbim.Ifc.Extensions;

namespace IFCConverter.Exporter.StartToDomain.EntityMetadataAugmenters
{
    internal sealed class EntityMetadataAugmenter : IEntityMetadataAugmenter
    {
        public bool CanResolve(IStartEntity source, StartMappingContext context)
        {
            return context.TryGetEntityId(source, out _);
        }

        public void Augment(IStartEntity source, EngineeringModel model, StartMappingContext context)
        {
            Entity entity = model.GetEntity(context.GetEntityId(source));
            StartElementTypeEnum startType = source.GetStartElementAttribute().Type;

            entity.Metadata.Type = startType.ToString();
            entity.Metadata.Name = GenerateName(source);
        }

        private static string GenerateName(IStartEntity source)
        {
            if (!source.Name.IsEmpty())
                return source.Name;
            
            switch (source)
            {
                case IStartOneNodeEntity oneNodeEntity:
                    return GenerateOneNodeEntityName(oneNodeEntity);
                case IStartTwoNodeEntity twoNodeEntity:
                    return GenerateTwoNodeEntityName(twoNodeEntity);
                default:
                    throw new InvalidOperationException($"Cannot generate name for {source.GetType().FullName} type");
            }
        }

        private static string GenerateOneNodeEntityName(IStartOneNodeEntity oneNodeEntity)
        {
            return $"{oneNodeEntity.GetType().Name}_{oneNodeEntity.Node.Name}";
        }
        
        private static string GenerateTwoNodeEntityName(IStartTwoNodeEntity twoNodeEntity)
        {
            return $"{twoNodeEntity.GetType().Name}_{twoNodeEntity.StartNode.Name}_{twoNodeEntity.EndNode.Name}";
        }
    }
}