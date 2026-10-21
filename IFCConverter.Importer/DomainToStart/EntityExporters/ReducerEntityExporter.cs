using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Start.Entities.Fittings;

namespace IFCConverter.Importer.DomainToStart.EntityExporters
{
    internal sealed class ReducerEntityExporter : IEntityExporter
    {
        private Reducer _reducer;
        
        public bool CanExport(Entity entity)
        {
            if (!(entity is Reducer reducer))
                return false;
            _reducer = reducer;

            return true;
        }

        public void Export(Entity entity, EngineeringModel model, ExportContext context)
        {
            StartAbstractReducerEntity startReducer = _reducer.TypedMetadata.IsEccentric
                ? (StartAbstractReducerEntity)new StartReducerEccentricEntity()
                : new StartReducerConcentricEntity();

            startReducer.LengthOfConicalPart.CreateFromSI(_reducer.Length);
            startReducer.Position = _reducer.Position;
            startReducer.Name = _reducer.Metadata.Name;
            
            context.Register(_reducer, startReducer);
        }
    }
}