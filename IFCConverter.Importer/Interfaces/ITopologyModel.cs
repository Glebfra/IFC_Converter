using System.Collections.Generic;

namespace IFCConverter.Importer.Interfaces
{
    internal interface ITopologyModel
    {
        public IEnumerable<ITopologyEntity> Entities { get; }

        public void AddEntity(ITopologyEntity entity);
        public void AddEntities(IEnumerable<ITopologyEntity> entities);
    }
}