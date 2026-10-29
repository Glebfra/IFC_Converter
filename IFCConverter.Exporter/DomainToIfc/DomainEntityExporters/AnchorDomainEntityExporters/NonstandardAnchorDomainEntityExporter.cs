using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using IFCConverter.Domain.Entities;
using IFCConverter.IFC.API;
using IFCConverter.IFC.Builders.Elements;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using IFCConverter.Start.API;
using IFCConverter.Utils.Mathematics;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class NonstandardAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(StartElementTypeEnum type)
        {
            return type == StartElementTypeEnum.NONSTANDARD_RESTRAINT;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            FixedMatrix<Dim4> segmentMatrix = anchor.TypedMetadata.SegmentMatrix;
            double diameter = anchor.Port.Metadata.Diameter;

            List<FixedVector<Dim3>> positions = new List<FixedVector<Dim3>>(anchor.Restraints.Count);
            List<FixedVector<Dim3>> directions = new List<FixedVector<Dim3>>(anchor.Restraints.Count);

            for (int i = 0; i < anchor.Restraints.Count; i++)
            {
                AnchorRestraint restraint = anchor.Restraints[i];
                directions.Add(restraint.Direction);
                positions.Add(CalculatePosition(segmentMatrix, restraint.Direction, diameter));

                if (restraint.IsDoubleSided)
                {
                    directions.Add(restraint.Direction.Negate());
                    positions.Add(CalculatePosition(segmentMatrix, restraint.Direction.Negate(), diameter));
                }
            }

            IIfcGeometry geometry = NonstandardAnchorGeometry.CreateGeometry(model, new NonstandardAnchorGeometryProperties
            {
                Diameter = diameter,
                Positions = positions.ToArray(),
                Directions = directions.ToArray()
            });
            geometry.AssignColor(Color.FromHEX(anchor.Metadata.Color));

            FixedMatrix<Dim4> placement = FixedMatrix<Dim4>.Builder.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.NOTDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);
            builder.AssignObjectType(anchor.Metadata.Type);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(anchor, instance);
        }

        [Pure]
        private static FixedVector<Dim3> CalculatePosition(FixedMatrix<Dim4> segmentMatrix, FixedVector<Dim3> direction,
            double diameter)
        {
            if (direction.IsParallel(segmentMatrix.GetZ().ToFixedVectorDim3(), 1e-3))
                return segmentMatrix.GetY().ToFixedVectorDim3() * (diameter / 2);

            return direction.Negate() * MathExtensions.CalculateAnchorDisplacement(segmentMatrix, diameter);
        }
    }
}