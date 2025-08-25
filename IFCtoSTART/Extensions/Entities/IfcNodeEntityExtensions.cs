using IFC.Entities;
using Start.Entities;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace IFCtoSTART.Extensions.Entities
{
    internal static class IfcNodeEntityExtensions
    {
        private static int _id;
        
        public static StartNodeEntity ToStartEntity(this IfcNodeEntity ifcNodeEntity)
        {
            StartNodeEntity startNodeEntity = new StartNodeEntity();
            startNodeEntity.ID = _id++;

            XbimVector3D coordinates = ifcNodeEntity.ObjectMatrix3D.Translation;
            startNodeEntity.XCoord = LengthProperty.CreateFromSi(coordinates.X);
            startNodeEntity.YCoord = LengthProperty.CreateFromSi(coordinates.Y);
            startNodeEntity.ZCoord = LengthProperty.CreateFromSi(coordinates.Z);

            return startNodeEntity;
        }
    }
}