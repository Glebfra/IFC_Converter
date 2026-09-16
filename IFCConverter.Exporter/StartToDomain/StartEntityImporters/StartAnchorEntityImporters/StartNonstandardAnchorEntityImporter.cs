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
                    Direction = CalculateDirection(nonstandardStart, startNonStandardRestraintModule)
                };

                anchor.Restraints.Add(restraint);
            }

            model.Add(anchor);
            context.Register(start, anchor);
        }

        private static FixedVector<Dim3> CalculateDirection(StartNonstandardAnchorEntity start, StartNonStandardRestraintModule module)
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
                return FixedVector<Dim3>.Builder.Dense(restraintX, restraintY, restraintZ);

            foreach (IStartSegmentEntity segmentEntity in segmentEntities)
            {
                StartNodeEntity[] nodeEntities = segmentEntity.ConnectedEntities.OfType<StartNodeEntity>().ToArray();
                StartNodeEntity startNode = nodeEntities.FirstOrDefault(item => item.ID == start.SectionStartNode);
                StartNodeEntity endNode = nodeEntities.FirstOrDefault(item => item.ID == start.SectionEndNode);
                if (startNode == null || endNode == null)
                    continue;

                FixedVector<Dim3> direction = endNode.Position - startNode.Position;

                FixedVector<Dim3> zAxis = direction;
                FixedVector<Dim3> xAxis = zAxis.CreateNormalVector();
                FixedVector<Dim3> yAxis = zAxis.CreateNormalVector(xAxis);

                FixedMatrix<Dim4> transitionMatrix = FixedMatrix<Dim4>.Builder.CreateTransition(FixedVector<Dim3>.Zeros(), xAxis, yAxis, zAxis);
                return transitionMatrix.GetZ().ToCartesian() * restraintX +
                       transitionMatrix.GetX().ToCartesian() * restraintY +
                       transitionMatrix.GetY().ToCartesian() * restraintZ;
            }

            throw new Exception("Cannot calculate direction for anchor restraint module");
        }
    }
}