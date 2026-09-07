using System.Diagnostics.Contracts;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using IFCConverter.Start.Entities.Fittings;
using IFCConverter.Start.Extensions;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.Extensions
{
    internal static class StartEntityExtensions
    {
        [Pure]
        public static Vector<double>[] GetBotConePoints(this StartValveEntity valveEntity)
        {
            IStartSegmentEntity[] startSegmentEntities = valveEntity.ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .ToArray();
            return new[]
            {
                startSegmentEntities[0].GetNearestPosition(valveEntity.Position), startSegmentEntities[1].GetNearestPosition(valveEntity.Position)
            };
        }
    }
}