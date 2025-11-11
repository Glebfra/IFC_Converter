using IFC.Entities;

namespace IFCConverter.Extensions.Tools
{
    public static class StartToIfcNaming
    {
        public static string GenerateName(string name, string type, IfcNodeEntity[] nodeEntities)
        {
            return name != string.Empty ? name : $"{type} {nodeEntities[0].ID}-{nodeEntities[1].ID}";
        }

        public static string? GenerateName(string name, string type, IfcNodeEntity nodeEntity)
        {
            return name != string.Empty ? name : $"{type} {nodeEntity.ID}";
        }
    }
}