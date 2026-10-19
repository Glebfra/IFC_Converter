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
            
            StartAbstractReducerEntity startReducer = reducer.TypedMetadata.IsEccentric
                ? (StartAbstractReducerEntity)new StartReducerEccentricEntity()
                : new StartReducerConcentricEntity();

            startReducer.LengthOfConicalPart.CreateFromSI(reducer.Length);
            startReducer.Position = reducer.Position;
            
            context.Register(reducer, startReducer);
        }
    }
}