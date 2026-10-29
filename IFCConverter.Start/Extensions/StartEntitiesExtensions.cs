using System;
using System.Diagnostics.Contracts;
using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.Extensions
{
    public static class StartEntitiesExtensions
    {
        private static readonly StartElementsRegistry ElementsRegistry = new StartElementsRegistry();

        [Pure]
        public static StartElementAttribute GetStartElementAttribute(this IStartEntity entity)
        {
            return ElementsRegistry.GetStartElementAttribute(entity);
        }

        [Pure]
        public static StartElementTypeEnum GetElementType(this IStartEntity entity)
        {
            return ElementsRegistry.GetElementType(entity);
        }

        [Pure]
        public static FixedVector<Dim3> GetProjectionFromPoint(this IStartSegmentEntity segmentEntity,
            FixedVector<Dim3> position)
        {
            return segmentEntity.IsStartPosition(position)
                ? segmentEntity.Projection.SafeNormalize()
                : segmentEntity.Projection.SafeNormalize().Negate();
        }

        [Pure]
        public static FixedVector<Dim3> GetNearestPosition(this IStartEntity startEntity, FixedVector<Dim3> position)
        {
            switch (startEntity)
            {
                case IStartOneNodeEntity oneNodeEntity:
                    return oneNodeEntity.Position;
                case IStartTwoNodeEntity twoNodeEntity:
                    return twoNodeEntity.IsStartPosition(position) ? twoNodeEntity.StartPosition : twoNodeEntity.EndPosition;
                default:
                    throw new ArgumentException($"Unsupported entity type {startEntity.GetType().Name}");
            }
        }
    }
}