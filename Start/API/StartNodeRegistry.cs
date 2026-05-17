using System.Collections.Generic;
using System.Linq;
using Start.Entities;
using Start.Extensions;
using Start.Interfaces;
using Utils;

namespace Start.API
{
    public sealed class StartNodeRegistry
    {
        private readonly Dictionary<VectorKey, StartEntityProxy> _nodes =
            new Dictionary<VectorKey, StartEntityProxy>();

        private int _counter = 1;

        public StartEntityProxy[] GetOrCreateNodes(IStartProject startProject, IStartEntity startEntity)
        {
            return startEntity.GetPositions()
                .Select(pos => _nodes.GetOrAdd(new VectorKey(pos), key =>
                    {
                        StartNodeEntity nodeEntity = new StartNodeEntity() { Position = key.Coordinates };
                        StartEntityProxy proxy = startProject.AddEntity(nodeEntity);
                        proxy.StartBaseRoot.SetName((_counter++).ToString());
                        return proxy;
                    })
                ).ToArray();
        }
    }
}