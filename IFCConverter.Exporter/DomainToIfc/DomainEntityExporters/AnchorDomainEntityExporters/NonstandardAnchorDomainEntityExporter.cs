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
using MathNet.Numerics.LinearAlgebra;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.DomainToIfc.DomainEntityExporters.AnchorDomainEntityExporters
{
    internal sealed class NonstandardAnchorDomainEntityExporter : IAnchorDomainEntityExporter
    {
        public bool CanExport(Anchor anchor)
        {
            if (!Enum.TryParse(anchor.Metadata.Type, out StartElementTypeEnum type))
                return false;

            return type == StartElementTypeEnum.NONSTANDARD_RESTRAINT;
        }

        public void Export(Anchor anchor, IModel model, ExportContext context)
        {
            Matrix<double> segmentMatrix = (Matrix<double>)anchor.Metadata.Meta["SegmentMatrix"];
            double diameter = anchor.Port.Metadata.Diameter;
            
            List<Vector<double>> positions = new List<Vector<double>>(anchor.Restraints.Count);
            List<Vector<double>> directions = new List<Vector<double>>(anchor.Restraints.Count);

            for (int i = 0; i < anchor.Restraints.Count; i++)
            {
                AnchorRestraint restraint = anchor.Restraints[i];
                directions.Add(restraint.Direction);
                positions.Add(CalculatePosition(segmentMatrix, restraint.Direction, diameter));

                if (restraint.IsDoubleSided)
                {
                    directions.Add(-restraint.Direction);
                    positions.Add(CalculatePosition(segmentMatrix, -restraint.Direction, diameter));
                }
            }
            
            IIfcGeometry geometry = NonstandardAnchorGeometry.CreateGeometry(model, new NonstandardAnchorGeometryProperties()
            {
                Diameter = diameter,
                Positions = positions.ToArray(),
                Directions = directions.ToArray(),
            });
            geometry.AssignColor(Color.FromHEX(anchor.Metadata.Color));
            
            Matrix<double> placement = MatrixExtensions.CreateTransition(anchor.Position);
            IIfcDiscreteAccessoryBuilder<IIfcDiscreteAccessory> builder =
                new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(anchor.Metadata.Name, anchor.Metadata.Type, IfcDiscreteAccessoryTypeEnum.NOTDEFINED);
            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(model, placement);

            IIfcProduct instance = builder.CreateInstance(model);
            context.Register(anchor, instance);
        }
        
        [Pure]
        private static Vector<double> CalculatePosition(Matrix<double> segmentMatrix, Vector<double> direction,
            double diameter)
        {
            if (direction.IsParallel(segmentMatrix.GetZ(), 1e-3))
                return segmentMatrix.GetY() * diameter / 2;

            return -direction * MathExtensions.CalculateAnchorDisplacement(segmentMatrix, diameter);
        }
    }
}