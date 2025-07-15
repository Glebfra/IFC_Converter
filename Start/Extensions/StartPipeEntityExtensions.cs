using Start.Entities.Segments;
using Start.StartProperties;
using Xbim.Common.Geometry;

namespace Start.Extensions
{
    public static class StartPipeEntityExtensions
    {
        public static XbimVector3D GetProjection(this StartPipeEntity startPipeEntity)
        {
            return new XbimVector3D(
                startPipeEntity.ProjectionAlongOXAxis.SIProperty,
                startPipeEntity.ProjectionAlongOYAxis.SIProperty,
                startPipeEntity.ProjectionAlongOZAxis.SIProperty
            );
        }

        public static void SetProjection(this StartPipeEntity startPipeEntity, XbimVector3D projection)
        {
            startPipeEntity.ProjectionAlongOXAxis = LengthProperty.CreateFromSi(projection.X);
            startPipeEntity.ProjectionAlongOYAxis = LengthProperty.CreateFromSi(projection.Y);
            startPipeEntity.ProjectionAlongOZAxis = LengthProperty.CreateFromSi(projection.Z);
        }
    }
}