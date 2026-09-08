using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.Entities.Segments;
using IFCConverter.Start.Interfaces;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartSegmentEntityImporters
{
    internal static class SegmentDiameterFinder
    {
        private const double PipeDiameterFactor = 1.0 / 2;
        private const double CylindricalShellDiameterFactor = 1.0 / 20;
        
        public static double? GetDiameter(IStartSegmentEntity start)
        {
            HashSet<IStartSegmentEntity> visited = new HashSet<IStartSegmentEntity>();
            Queue<IStartSegmentEntity> queue = new Queue<IStartSegmentEntity>();
            
            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                IStartSegmentEntity element = queue.Dequeue();
                if (!visited.Add(element))
                    continue;

                switch (element)
                {
                    case StartCylindricalShellEntity _:
                    case StartPipeEntity _:
                        return GetDiameter(start, element);
                }
                
                foreach (IStartSegmentEntity startSegmentEntity in element.ConnectedEntities.OfType<IStartSegmentEntity>())
                {
                    if (!visited.Contains(startSegmentEntity))
                        queue.Enqueue(startSegmentEntity);
                }
            }

            return null;
        }

        private static double GetDiameter(IStartSegmentEntity start, IStartSegmentEntity element)
        {
            if (start is StartFlexibleElementEntity)
                return element.Diameter.SIProperty;

            switch (element)
            {
                case StartPipeEntity _:
                    return element.Diameter.SIProperty * PipeDiameterFactor;
                case StartCylindricalShellEntity _:
                    return element.Diameter.SIProperty * CylindricalShellDiameterFactor;
            }

            throw new NullReferenceException($"Cannot get diameter for {start.StartNode.Name}_{start.EndNode.Name}");
        }
    }
}