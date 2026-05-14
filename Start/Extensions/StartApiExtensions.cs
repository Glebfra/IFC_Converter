using Newtonsoft.Json;
using Start.API;
using Start.Interfaces;

namespace Start.Extensions
{
    public static class StartApiExtensions
    {
        public static StartEntityProxy AddEntity(this IStartProject startProject, IStartEntity entity)
        {
            string entityJson = JsonConvert.SerializeObject(entity);
            StartElementTypeEnum startElementType = entity.GetElementType();
            IStartBaseRoot startBaseRoot = startProject.AddElement(startElementType, out int index);
            startBaseRoot.SetDataJson(0, entityJson);

            return new StartEntityProxy(startBaseRoot, index);
        }
    }
}