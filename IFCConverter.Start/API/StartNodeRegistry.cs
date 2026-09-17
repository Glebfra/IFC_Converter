using System.Collections.Generic;
using System.Linq;
using IFCConverter.Start.Entities;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Collections;
using IFCConverter.Utils.Mathematics;

namespace IFCConverter.Start.API
{
    public sealed class StartNodeRegistry
    {
        private readonly Dictionary<FixedVector<Dim3>, StartEntityProxy> _nodes;
        private int _counter = 1;

        public StartNodeRegistry(VectorComparer comparer)
        {
            _nodes = new Dictionary<FixedVector<Dim3>, StartEntityProxy>(comparer);
        }

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, params FixedVector<Dim3>[] positions)
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