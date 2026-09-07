using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Collections;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Start.Extensions
{
    public static class StartEntitiesExtensions
    {
        private const double EQUALS_TOLERANCE = 1e-6;

        private static readonly Dictionary<Type, StartElementTypeEnum> _elementTypesCache = new Dictionary<Type, StartElementTypeEnum>();
        private static readonly Dictionary<Type, StartElementAttribute> StartElementCache = new Dictionary<Type, StartElementAttribute>();

        [Pure]
        public static StartElementAttribute GetStartElementAttribute(this IStartEntity entity)
        {
            Type type = entity.GetType();
            return StartElementCache.GetOrAdd(type, t => t.GetCustomAttribute<StartElementAttribute>());
        }

        [Pure]
        public static bool IsConnectedTo(this IStartEntity startEntity, IStartEntity otherEntity)
        {
            IEnumerable<Vector<double>> startEntityPositions = startEntity.GetPositions();
            IEnumerable<Vector<double>> otherEntityPositions = otherEntity.GetPositions();

            return (
                from startEntityPosition in startEntityPositions
                from otherEntityPosition in otherEntityPositions
                where startEntityPosition.AlmostEqual(otherEntityPosition, EQUALS_TOLERANCE)
                select startEntityPosition
            ).Any();
        }

        [Pure]
        public static IEnumerable<Vector<double>> GetPositions(this IStartEntity startEntity)
        {
            switch (startEntity)
            {
                case IStartOneNodeEntity oneNodeEntity:
                    return new[]
                    {
                        oneNodeEntity.Position
                    };
                case IStartTwoNodeEntity twoNodeEntity:
                    return new[]
                    {
                        twoNodeEntity.StartPosition, twoNodeEntity.EndPosition
                    };
                default:
                    throw new Exception("Unsupported type");
            }
        }

        [Pure]
        public static IEnumerable<T> GetConnectedEntities<T>(this IStartEntity startEntity,
            IEnumerable<T> otherEntities)
            where T : IStartEntity
        {
            return otherEntities.Where(entity => startEntity.IsConnectedTo(entity));
        }

        [Pure]
        public static Vector<double> GetDirectionToEntity(this IStartEntity startEntity, Vector<double> position)
        {
            return startEntity.GetNearestPosition(position) - position;
        }

        [Pure]
        public static Vector<double> GetProjectionFromPoint(this IStartSegmentEntity segmentEntity,
            Vector<double> position)
        {
            return segmentEntity.IsStartPosition(position)
                ? segmentEntity.Projection.Normalize(2)
                : segmentEntity.Projection.Normalize(2).Negate();
        }

        [Pure]
        public static Vector<double> GetNearestPosition(this IStartEntity startEntity, Vector<double> position)
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

        [Pure]
        public static StartElementTypeEnum GetElementType(this IStartEntity entity)
        {
            return _elementTypesCache.GetOrAdd(
                entity.GetType(),
                type => type.GetCustomAttribute<StartElementAttribute>().Type
            );
        }
    }
}