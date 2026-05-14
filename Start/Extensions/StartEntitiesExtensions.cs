using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;
using Start.API;
using Start.Attributes;
using Start.Interfaces;

namespace Start.Extensions
{
    public static class StartEntitiesExtensions
    {
        private const double EQUALS_TOLERANCE = 1e-6;

        [Pure]
        public static bool IsConnectedTo(this IStartEntity startEntity, IStartEntity otherEntity)
        {
            IEnumerable<Vector<double>>? startEntityPositions = startEntity.GetPositions();
            IEnumerable<Vector<double>>? otherEntityPositions = otherEntity.GetPositions();
            if (startEntityPositions == null || otherEntityPositions == null)
                return false;
            
            return (
                from startEntityPosition in startEntityPositions
                from otherEntityPosition in otherEntityPositions
                where startEntityPosition.AlmostEqual(otherEntityPosition, EQUALS_TOLERANCE)
                select startEntityPosition
            ).Any();
        }

        [Pure]
        public static IEnumerable<Vector<double>>? GetPositions(this IStartEntity startEntity)
        {
            return startEntity switch
            {
                IStartOneNodeEntity oneNodeEntity => new[] { oneNodeEntity.Position },
                IStartTwoNodeEntity twoNodeEntity => new[] { twoNodeEntity.StartPosition, twoNodeEntity.EndPosition },
                _ => null
            };
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
            return GetNearestPosition(startEntity, position) - position;
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
            return startEntity switch
            {
                IStartOneNodeEntity oneNodeEntity => oneNodeEntity.Position,
                IStartTwoNodeEntity twoNodeEntity => twoNodeEntity.IsStartPosition(position)
                    ? twoNodeEntity.StartPosition
                    : twoNodeEntity.EndPosition,
                _ => throw new ArgumentException($"Unsupported entity type {startEntity.GetType().Name}")
            };
        }

        [Pure]
        public static StartElementTypeEnum GetElementType(this IStartEntity entity)
        {
            return entity.GetType().GetCustomAttribute<StartElementAttribute>().Type;
        }
    }
}