using System.Collections.Generic;

namespace Start.Entities
{
    public struct GroupedEntities
    {
        public Dictionary<int, StartAbstractEntity> NodeEntities;
        public Dictionary<int, StartAbstractEntity> PipeEntities;
        public Dictionary<int, StartAbstractEntity> FittingEntities;
        public Dictionary<int, int[]> PipeNodeRelations;
        public Dictionary<int, int> FittingNodeRelations;
    }
}