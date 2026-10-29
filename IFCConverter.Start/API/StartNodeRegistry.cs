using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.Entities;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.API
{
    public sealed class StartNodeRegistry
    {
        private readonly Dictionary<Cell, List<NodeEntry>> _nodes = new Dictionary<Cell, List<NodeEntry>>();
        private readonly double _tolerance;

        private int _counter = 1;

        public StartNodeRegistry(double tolerance)
        {
            if (double.IsNaN(tolerance) ||
                double.IsInfinity(tolerance) ||
                tolerance <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(tolerance));
            }

            _tolerance = tolerance;
        }

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, params FixedVector<Dim3>[] positions)
        {
            if (startProject == null)
                throw new ArgumentNullException(nameof(startProject));

            if (positions == null)
                throw new ArgumentNullException(nameof(positions));

            return positions
                .Select(position => GetOrCreateNode(startProject, position))
                .ToArray();
        }

        private StartEntityProxy GetOrCreateNode(IStartProject startProject, FixedVector<Dim3> position)
        {
            if (position == null)
                throw new ArgumentNullException(nameof(position));

            Cell cell = GetCell(position);

            for (int dx = -1; dx <= 1; dx++)
            for (int dy = -1; dy <= 1; dy++)
            for (int dz = -1; dz <= 1; dz++)
            {
                Cell neighbor = new Cell(cell.X + dx, cell.Y + dy, cell.Z + dz);

                List<NodeEntry> entries;
                if (!_nodes.TryGetValue(neighbor, out entries))
                    continue;

                foreach (NodeEntry entry in entries)
                {
                    if (AreEqual(entry.Position, position))
                        return entry.Proxy;
                }
            }

            StartNodeEntity nodeEntity = new StartNodeEntity
            {
                Position = position
            };

            StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
            proxy.StartBaseRoot.SetName((_counter++).ToString());

            List<NodeEntry> bucket;

            if (!_nodes.TryGetValue(cell, out bucket))
            {
                bucket = new List<NodeEntry>();
                _nodes.Add(cell, bucket);
            }

            bucket.Add(new NodeEntry(position, proxy));

            return proxy;
        }

        private bool AreEqual(FixedVector<Dim3> a, FixedVector<Dim3> b)
        {
            return Math.Abs(a[0] - b[0]) <= _tolerance &&
                   Math.Abs(a[1] - b[1]) <= _tolerance &&
                   Math.Abs(a[2] - b[2]) <= _tolerance;
        }

        private Cell GetCell(FixedVector<Dim3> position)
        {
            return new Cell(ToCellCoordinate(position[0]), ToCellCoordinate(position[1]), ToCellCoordinate(position[2]));
        }

        private long ToCellCoordinate(double value)
        {
            if (double.IsNaN(value) || double.IsInfinity(value))
                throw new ArgumentOutOfRangeException(nameof(value));

            double coordinate = Math.Floor(value / _tolerance);

            if (coordinate < long.MinValue ||
                coordinate >= 9223372036854775808.0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            return (long)coordinate;
        }

        private sealed class NodeEntry
        {

            public NodeEntry(FixedVector<Dim3> position, StartEntityProxy proxy)
            {
                Position = position;
                Proxy = proxy;
            }

            public FixedVector<Dim3> Position { get; }
            public StartEntityProxy Proxy { get; }
        }

        private struct Cell : IEquatable<Cell>
        {
            public readonly long X;
            public readonly long Y;
            public readonly long Z;

            public Cell(long x, long y, long z)
            {
                X = x;
                Y = y;
                Z = z;
            }

            public bool Equals(Cell other)
            {
                return X == other.X && Y == other.Y && Z == other.Z;
            }

            public override bool Equals(object obj)
            {
                return obj is Cell && Equals((Cell)obj);
            }

            public override int GetHashCode()
            {
                unchecked
                {
                    int hash = 17;
                    hash = hash * 23 + X.GetHashCode();
                    hash = hash * 23 + Y.GetHashCode();
                    hash = hash * 23 + Z.GetHashCode();
                    return hash;
                }
            }
        }
    }
}