using System;
using System.Net.NetworkInformation;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedBldgElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters
{
    internal sealed class BeamDomainEntityExporter : IDomainEntityExporter
    {
        public bool CanExport(Entity entity)
        {
            return entity is Beam;
        }

        public void Export(Entity entity, IModel model, ExportContext context)
        {
            Beam beam = (Beam)entity;
            
            Matrix<double> transformation = (Matrix<double>)beam.Metadata.Meta["TransformationMatrix"];
            Matrix<double> ma = MatrixExtensions.CreateRotationAroundVector(transformation.GetZ(), beam.SectionAxisAngle);
            Vector<double> refDirection = ma.GetRotation().LeftMultiply(transformation.GetY());
            
            Vector<double> projection = beam.EndPort.Position - beam.StartPort.Position;
            double length = projection.L2Norm();
            Vector<double> direction = projection / length;

            if (!Enum.TryParse((string)entity.Metadata.Meta["BeamType"], out StartBeamTypeEnum type))
                throw new InvalidOperationException($"Cannot find type for {entity.Id}");
            
            IIfcGeometry geometry = BeamGeometry.CreateGeometry(model, new BeamGeometryProperties()
            {
                Position = VectorExtensions.Zero,
                Direction = direction,
                RefDirection = refDirection,
                Length = length,
                Height = beam.Height,
                Width = beam.Width,
                GeometryType = CreateGeometryType(type),
                Diameter = beam.Diameter
            });
            geometry.AssignColor(Color.FromHEX(entity.Metadata.Color));

            Matrix<double> placement = MatrixExtensions.CreateTransition(beam.StartPort.Position);
            IIfcBeamBuilder<IIfcBeam> builder = new IfcBeamBuilder<IfcBeam>(entity.Metadata.Name, entity.Metadata.Type, IfcBeamTypeEnum.BEAM);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(entity, instance);
        }
        
        private static BendGeometryType CreateGeometryType(StartBeamTypeEnum type)
        {
            switch (type)
            {
                case StartBeamTypeEnum.NONSTANDARD:
                case StartBeamTypeEnum.IBEAM:
                    return BendGeometryType.IBEAM;
                case StartBeamTypeEnum.CHANNEL:
                    return BendGeometryType.CHANNEL;
                case StartBeamTypeEnum.TBEAM:
                    return BendGeometryType.TBEAM;
                case StartBeamTypeEnum.CORNERBEAM:
                    return BendGeometryType.CORNERBEAM;
                case StartBeamTypeEnum.BOXBEAM:
                    return BendGeometryType.RECTANGULARBEAM;
                case StartBeamTypeEnum.PIPEBEAM:
                case StartBeamTypeEnum.CIRCLEBEAM:
                    return BendGeometryType.CIRCLEBEAM;
                case StartBeamTypeEnum.RECTANGULARBEAM:
                    return BendGeometryType.RECTANGULARBEAM;
                default:
                    return BendGeometryType.IBEAM;
            }
        }
    }
}