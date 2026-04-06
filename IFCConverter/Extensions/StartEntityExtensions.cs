using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities.Fittings;
using Start.Extensions;
using Start.Interfaces;

namespace IFCConverter.Extensions
{
    internal static class StartEntityExtensions
    {
        public static Vector<double>[] GetBotConePoints(this StartValveEntity valveEntity)
        {
            IStartSegmentEntity[] startSegmentEntities = valveEntity.ConnectedEntities
                .OfType<IStartSegmentEntity>()
                .ToArray();
            return new Vector<double>[]
            {
                startSegmentEntities[0].GetNearestPosition(valveEntity.Position),
                startSegmentEntities[1].GetNearestPosition(valveEntity.Position)
            };
        }
    }
}