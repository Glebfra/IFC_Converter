using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyModel
    {
        public IReadOnlyCollection<ITopologyEntity> Entities { get; }
        public IReadOnlyCollection<ITopologyNodeEntity> Nodes { get; }
    }
}