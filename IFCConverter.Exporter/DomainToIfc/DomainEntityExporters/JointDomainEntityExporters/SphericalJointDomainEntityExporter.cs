using System;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using Xbim.Common;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.JointDomainEntityExporters
{
    internal sealed class BallJointDomainEntityExporter : AbstractJointDomainEntityExporter
    {
        public override bool CanExport(Joint joint)
        {
            if (!Enum.TryParse(joint.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.BALL_EXPANSION_JOINT || 
                   type == StartElementTypeEnum.ANGULAR_EXPANSION_JOINT;
        }

        override protected IIfcGeometry CreateGeometry(Joint joint, IModel model)
        {
            return BallExpansionJointGeometry.CreateGeometry(model, new BallExpansionJointGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Diameter = joint.Length * 2
            });
        }
    }
}