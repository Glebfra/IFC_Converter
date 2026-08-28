using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.Entities;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Collections;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;

namespace IFCConverter.Start.API
{
    public sealed class StartNodeRegistry
    {
        private readonly Dictionary<Vector<double>, StartEntityProxy> _nodes;
        private int _counter = 1;

        public StartNodeRegistry(VectorComparer comparer)
        {
            _nodes = new Dictionary<Vector<double>, StartEntityProxy>(comparer);
        }

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, params Vector<double>[] positions)
        {
            return positions
                .Select(position => _nodes.GetOrAdd(position, vector =>
                    {
                        StartNodeEntity nodeEntity = new StartNodeEntity
                        {
                            Position = vector
                        };
                        StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
                        proxy.StartBaseRoot.SetName((_counter++).ToString());
                        return proxy;
                    })
                ).ToArray();
        }
    }
}