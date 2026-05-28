using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Ifc.API;
using Ifc.Builders.Elements;
using Ifc.Geometries;
using Ifc.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Start.API;
using Start.Entities;
using Start.Entities.Anchors;
using Start.Interfaces;
using Utils;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.SharedComponentElements;
using MatrixExtensions = Utils.MatrixExtensions;
using VectorExtensions = Utils.VectorExtensions;

namespace IFCConverter.Exporter.Converters.Elements
{
    internal sealed class NonstandardAnchorConverter :
        IfcElementConverter<StartNonstandardAnchorEntity, IfcDiscreteAccessory>
    {
        public NonstandardAnchorConverter(IModel model) : base(model)
        {
        }

        public override IIfcGeometry CreateGeometry(StartNonstandardAnchorEntity start)
        {
            IStartSegmentEntity[] segmentEntities = start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            Matrix<double> segmentMatrix = segmentEntities[0].TransformationMatrix;
            double diameter = segmentEntities.Max(segmentEntity => segmentEntity.Diameter).SIProperty;

            StartNonStandardRestraintModule[] restraintModules = start.Restraints.ToArray();
            List<Vector<double>> positions = new(restraintModules.Length);
            List<Vector<double>> directions = new(restraintModules.Length);

            for (int i = 0; i < restraintModules.Length; i++)
            {
                StartNonStandardRestraintModule restraintModule = restraintModules[i];
                bool useLocalAxes = restraintModule.Local.EnumValue == StartRestraintAxesTypeEnum.LOCAL;
                Vector<double> direction = useLocalAxes
                    ? CreateDirectionFromLocal(start, restraintModule, segmentEntities)
                    : CreateDirection(restraintModule);
                Vector<double> position = CalculatePosition(segmentMatrix, direction, diameter);
                directions.Add(direction);
                positions.Add(position);

                if (restraintModule.Type.EnumValue == StartRestraintTypeEnum.RIGID_DOUBLE_SIDED)
                {
                    direction = -direction;
                    position = CalculatePosition(segmentMatrix, direction, diameter);
                    directions.Add(direction);
                    positions.Add(position);
                }
            }

            NonstandardAnchorGeometry geometry = NonstandardAnchorGeometry.CreateGeometry(_Model,
                new NonstandardAnchorGeometryProperties
                {
                    Diameter = diameter,
                    Directions = directions.ToArray(),
                    Positions = positions.ToArray()
                });
            geometry.AssignColor(Color.FromHEX("4ab636"));

            return geometry;
        }

        public override Matrix<double> CreateObjectMatrix(StartNonstandardAnchorEntity start)
        {
            return MatrixExtensions.CreateTransition(start.Position);
        }

        public override IIfcProductBuilder<IfcDiscreteAccessory> CreateBuilder(StartNonstandardAnchorEntity start)
        {
            return new IfcDiscreteAccessoryBuilder<IfcDiscreteAccessory>(
                GenerateName(start), GenerateTag(start), IfcDiscreteAccessoryTypeEnum.ANCHORPLATE
            );
        }

        public override StartNonstandardAnchorEntity BuildStartElement(IfcDiscreteAccessory ifc)
        {
            throw new NotImplementedException();
        }

        [Pure]
        private static Vector<double> CreateDirection(StartNonStandardRestraintModule restraintModule)
        {
            double restraintX = restraintModule.AngleX.SIProperty < 0
                ? -Math.Cos(restraintModule.AngleX.SIProperty)
                : Math.Cos(restraintModule.AngleX.SIProperty);
            double restraintY = restraintModule.AngleY.SIProperty < 0
                ? -Math.Cos(restraintModule.AngleY.SIProperty)
                : Math.Cos(restraintModule.AngleY.SIProperty);
            double restraintZ = restraintModule.AngleZ.SIProperty < 0
                ? -Math.Cos(restraintModule.AngleZ.SIProperty)
                : Math.Cos(restraintModule.AngleZ.SIProperty);
            return new DenseVector(new[] { restraintX, restraintY, restraintZ });
        }

        [Pure]
        private static Vector<double> CreateDirectionFromLocal(StartNonstandardAnchorEntity start,
            StartNonStandardRestraintModule restraintModule, IStartSegmentEntity[] segmentEntities)
        {
            double restraintX = restraintModule.AngleX.SIProperty < 0
                ? -Math.Cos(restraintModule.AngleX.SIProperty)
                : Math.Cos(restraintModule.AngleX.SIProperty);
            double restraintY = restraintModule.AngleY.SIProperty < 0
                ? -Math.Cos(restraintModule.AngleY.SIProperty)
                : Math.Cos(restraintModule.AngleY.SIProperty);
            double restraintZ = restraintModule.AngleZ.SIProperty < 0
                ? -Math.Cos(restraintModule.AngleZ.SIProperty)
                : Math.Cos(restraintModule.AngleZ.SIProperty);

            foreach (IStartSegmentEntity segmentEntity in segmentEntities)
            {
                StartNodeEntity[] nodeEntities = segmentEntity.ConnectedEntities.OfType<StartNodeEntity>().ToArray();
                StartNodeEntity? startNode = nodeEntities.FirstOrDefault(item => item.ID == start.SectionStartNode);
                StartNodeEntity? endNode = nodeEntities.FirstOrDefault(item => item.ID == start.SectionEndNode);
                if (startNode == null || endNode == null)
                    continue;

                Vector<double> direction = endNode.Position - startNode.Position;
                Matrix<double> transitionMatrix =
                    MatrixExtensions.CreateTransitionWithWorldUp(VectorExtensions.Zero, direction);
                return transitionMatrix.GetZ() * restraintX +
                       transitionMatrix.GetX() * restraintY +
                       transitionMatrix.GetY() * restraintZ;
            }

            throw new Exception("Cannot find local axes for nonstandard anchor restraint module");
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