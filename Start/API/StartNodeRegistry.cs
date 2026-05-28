using System;
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
                        StartNodeEntity nodeEntity = new StartNodeEntity() { Position = vector };
                        StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
                        proxy.StartBaseRoot.SetName((_counter++).ToString());
                        return proxy;
                    })
                ).ToArray();
        }

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, IStartEntity startEntity)
        {
            Vector<double>[] positions = startEntity.GetPositions().ToArray();
            return GetOrCreateNodes(startProject, positions);
        }
    }
}