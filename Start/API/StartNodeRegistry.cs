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

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, IStartEntity startEntity)
        {
            return startEntity.GetPositions()
                .Select(pos => _nodes.GetOrAdd(pos, key =>
                    {
                        StartNodeEntity nodeEntity = new StartNodeEntity() { Position = pos };
                        StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
                        proxy.StartBaseRoot.SetName((_counter++).ToString());
                        return proxy;
                    })
                ).ToArray();
        }
    }
}