using IFCConverter.Domain;

namespace IFCConverter.Importer.IfcToDomain.EntityConnectionsResolvers
{
    internal interface IEntityConnectionResolver
    {
        bool CanResolve();
        void Resolve(EngineeringModel model, ImportContext context);
    }
}