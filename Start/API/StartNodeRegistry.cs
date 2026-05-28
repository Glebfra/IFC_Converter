using System.Collections.Generic;
using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using Start.Entities;
using Start.Extensions;
using Start.Interfaces;
using Utils;

namespace Start.API
{
    public sealed class StartNodeRegistry
    {
        private readonly Dictionary<Vector<double>, StartEntityProxy> _nodes;
        private int _counter = 1;

        public StartNodeRegistry(double tolerance)
        {
            _nodes = new Dictionary<Vector<double>, StartEntityProxy>(new VectorComparer(tolerance));
        }

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, params Vector<double>[] positions)
        {
            return positions
                .Select(position => _nodes.GetOrAdd(position, vector =>
                    {
                        StartNodeEntity nodeEntity = new() { Position = vector };
                        StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
                        proxy.StartBaseRoot.SetName((_counter++).ToString());
                        return proxy;
                    })
                ).ToArray();
        }
    }
}