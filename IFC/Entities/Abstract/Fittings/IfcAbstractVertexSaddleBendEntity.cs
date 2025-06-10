using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using IFC.Tools.Geometry;
using IFC.Tools.Shape;
using Start.Entities.Fittings;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.RepresentationResource;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexSaddleBendEntity : IfcAbstractBendEntity
    {
        public abstract int NumSegments { get; protected set; }
        public abstract double BranchHeight { get; protected set; }
        public abstract double BranchPipeRadius { get; protected set; }
        
        protected IfcAbstractSegmentEntity[] _BranchPipes;
        protected IfcAbstractSegmentEntity _HeadPipe;
        
        private readonly StartBendEntity _bendEntity;
        private IfcPipeFitting? _pipeFitting;
        
        protected IfcAbstractVertexSaddleBendEntity(StartBendEntity bendEntity, IfcNodeEntity nodeEntity, IfcAbstractSegmentEntity[] segmentEntities) 
            : base(bendEntity, nodeEntity, segmentEntities)
        {
            _bendEntity = bendEntity;
            
            _BranchPipes = new IfcAbstractSegmentEntity[2];
            for (int i = 0; i < AbstractSegmentEntities.Length; i++)
            {
                for (int j = i + 1; j < AbstractSegmentEntities.Length; j++)
                {
                    XbimVector3D firstPipeDir = AbstractSegmentEntities[i].ObjectMatrix3D.Forward;
                    XbimVector3D secondPipeDir = AbstractSegmentEntities[j].ObjectMatrix3D.Forward;
                    
                    if (!firstPipeDir.IsParallel(secondPipeDir))
                        continue;
                    _BranchPipes[0] = AbstractSegmentEntities[i];
                    _BranchPipes[1] = AbstractSegmentEntities[j];
                    _HeadPipe = AbstractSegmentEntities[AbstractSegmentEntities.Length - (i + j)];
                }
            }
            if (_HeadPipe == null)
                throw new NullReferenceException("Cannot find head pipe");
            if (_BranchPipes == null)
                throw new NullReferenceException("Cannot find branch pipes");

            _BranchPipes = _BranchPipes.OrderByDescending(entity => entity.Diameter).ToArray();
            
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D forward = IfcAxis.GetPipeDirectionFromNode(_BranchPipes[1], coordinates).Normalized();
            XbimVector3D right = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, coordinates).Normalized();
            XbimVector3D up = XbimVector3D.CrossProduct(forward, right);
            ObjectMatrix3D = XbimMatrix3D.CreateWorld(coordinates, forward, up);
        }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            ObjectMatrix3D = ObjectMatrix3D.Translate(CalculateCircleCenter());
            
            IfcObjectPlacement objectPlacement = IfcAxis.CreatePointAndDirectionsObjectPlacement(model, ObjectMatrix3D);

            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            IfcAxisSettings axisSettings = new IfcAxisSettings(XbimVector3D.Zero, VectorExtensions.X, VectorExtensions.Y);
            representationItems.Add(IfcVertexGeometry.CreateTorus(model, BendRadius, PipeRadius, Angle, NumSegments, axisSettings));
            
            XbimVector3D branchDisplacement = BendRadius * Math.Tan(Angle / 2) * (VectorExtensions.Forward + VectorExtensions.Right);
            representationItems.Add(IfcGeometry.CreateCylinder(
                model, BranchPipeRadius, BranchHeight, branchDisplacement, VectorExtensions.Forward.Negated(), VectorExtensions.Right
            ));
            IfcShapeRepresentation shapeRepresentation = IfcGeometry.CreateShapeRepresentation(model, representationItems, IfcRepresentationType.SweptSolid, IfcRepresentationIdentifier.Body);
            IfcProductDefinitionShape shape = IfcGeometry.CreateProductDefinitionShape(model, shapeRepresentation);
            ColourEntity(model, representationItems);
            
            _pipeFitting = model.Instances.New<IfcPipeFitting>(fitting =>
            {
                fitting.Name = _bendEntity.Name;
                fitting.Tag = Tag;
                fitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;
                fitting.Representation = shape;
                fitting.ObjectPlacement = objectPlacement.LocalPlacement;
            });

            AddProperties(model, _pipeFitting);
            ClipConnectedPipes();

            return _pipeFitting;
        }

        protected override void ClipConnectedPipes()
        {
            double clipLength = BendRadius * Math.Tan(Angle / 2);
            _BranchPipes[0].Clip(NodeEntity, clipLength);
            _HeadPipe.Clip(NodeEntity, clipLength);
        }

        protected override XbimVector3D CalculateCircleCenter()
        {
            XbimVector3D coordinates = NodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D directionToHeadPipe = IfcAxis.GetPipeDirectionFromNode(_HeadPipe, coordinates);
            XbimVector3D directionToBranchPipe = IfcAxis.GetPipeDirectionFromNode(_BranchPipes[0], coordinates);
            XbimVector3D dirToCenter = (directionToBranchPipe + directionToHeadPipe).Normalized();
            double lengthToCenter = BendRadius / Math.Cos(Angle / 2);
            
            return dirToCenter * lengthToCenter;
        }
    }
}