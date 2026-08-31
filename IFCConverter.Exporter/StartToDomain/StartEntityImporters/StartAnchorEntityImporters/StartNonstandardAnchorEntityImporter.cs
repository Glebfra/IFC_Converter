using System;
using System.Linq;
using IFCConverter.Domain;
using IFCConverter.Domain.Entities;
using IFCConverter.Domain.Identity;
using IFCConverter.Start.API;
using IFCConverter.Start.Entities;
using IFCConverter.Start.Entities.Anchors;
using IFCConverter.Start.Interfaces;
using IFCConverter.Utils.Mathematics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using VectorExtensions = IFCConverter.Utils.Mathematics.VectorExtensions;
using MatrixExtensions = IFCConverter.Utils.Mathematics.MatrixExtensions;

namespace IFCConverter.Exporter.StartToDomain.StartEntityImporters.StartAnchorEntityImporters
{
    internal sealed class StartNonstandardAnchorEntityImporter : IStartAnchorEntityImporter
    {
        public bool CanImport(StartAbstractAnchorEntity start)
        {
            return start is StartNonstandardAnchorEntity;
        }

        public void Import(StartAbstractAnchorEntity start, EngineeringModel model, StartMappingContext context)
        {
            StartNonstandardAnchorEntity nonstandardStart = (StartNonstandardAnchorEntity)start;
            
            Anchor anchor = new Anchor(EntityId.New())
            {
                Position = start.Position
            };
            
            foreach (StartNonStandardRestraintModule startNonStandardRestraintModule in nonstandardStart.Restraints)
            {
                AnchorRestraint restraint = new AnchorRestraint
                {
                    IsDoubleSided = startNonStandardRestraintModule.Type.EnumValue == StartRestraintTypeEnum.RIGID_DOUBLE_SIDED,
                    Direction = CalculateDirection(nonstandardStart, startNonStandardRestraintModule),
                };

                anchor.Restraints.Add(restraint);
            }

            model.Add(anchor);
            context.Register(start, anchor);
        }

        private static Vector<double> CalculateDirection(StartNonstandardAnchorEntity start, StartNonStandardRestraintModule module)
        {
            IStartSegmentEntity[] segmentEntities = start.ConnectedEntities.OfType<IStartSegmentEntity>().ToArray();
            
            double restraintX = module.AngleX.SIProperty < 0
                ? -Math.Cos(module.AngleX.SIProperty)
                : Math.Cos(module.AngleX.SIProperty);
            double restraintY = module.AngleY.SIProperty < 0
                ? -Math.Cos(module.AngleY.SIProperty)
                : Math.Cos(module.AngleY.SIProperty);
            double restraintZ = module.AngleZ.SIProperty < 0
                ? -Math.Cos(module.AngleZ.SIProperty)
                : Math.Cos(module.AngleZ.SIProperty);
            
            if (module.Local.EnumValue == StartRestraintAxesTypeEnum.NOT_LOCAL)
                return new DenseVector(new double[] { restraintX, restraintY, restraintZ });
            
            foreach (IStartSegmentEntity segmentEntity in segmentEntities)
            {
                StartNodeEntity[] nodeEntities = segmentEntity.ConnectedEntities.OfType<StartNodeEntity>().ToArray();
                StartNodeEntity startNode = nodeEntities.FirstOrDefault(item => item.ID == start.SectionStartNode);
                StartNodeEntity endNode = nodeEntities.FirstOrDefault(item => item.ID == start.SectionEndNode);
                if (startNode == null || endNode == null)
                    continue;

                Vector<double> direction = endNode.Position - startNode.Position;
                Matrix<double> transitionMatrix =
                    MatrixExtensions.CreateTransitionWithWorldUp(VectorExtensions.Zero, direction);
                return transitionMatrix.GetZ() * restraintX +
                       transitionMatrix.GetX() * restraintY +
                       transitionMatrix.GetY() * restraintZ;
            }
            
            throw new Exception("Cannot calculate direction for anchor restraint module");
        }
    }
}