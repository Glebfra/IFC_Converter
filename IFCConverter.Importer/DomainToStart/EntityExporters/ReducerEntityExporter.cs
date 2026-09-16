using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class ReducerEntityExporter : IEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Reducer;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            Reducer reducer = (Reducer)entity;
            
            bool isEccentric = false;
            if (entity.Metadata.Meta.TryGetValue("IsEccentric", out object isEccentricValue))
                isEccentric = (bool)isEccentricValue;

            StartAbstractReducerEntity startReducer = isEccentric
                ? (StartAbstractReducerEntity)new StartReducerEccentricEntity()
                : new StartReducerConcentricEntity();

            startReducer.LengthOfConicalPart.CreateFromSI((reducer.PortB.Position - reducer.PortA.Position).L2Norm());
            startReducer.Position = reducer.Position;
        }
    }
}