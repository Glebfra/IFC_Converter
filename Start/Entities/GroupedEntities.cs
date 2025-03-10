using System.Collections.Generic;

namespace Start.Entities
{
    public struct GroupedEntities
    {
        public StartEntityContainer[] twoNodeEntitiesContainers;
        public StartEntityContainer[] oneNodeEntitiesContainers;
        public StartEntityContainer[] nodeEntitiesContainers;
        public Dictionary<int, StartAbstractEntity> NodeEntities;
        public Dictionary<int, StartAbstractEntity> TwoNodeEntities;
        public Dictionary<int, StartAbstractEntity> OneNodeEntities;
        public Dictionary<int, int[]> TwoNodeEntitiesRelations;
        public Dictionary<int, int> OneNodeEntitiesRelations;
    }
}