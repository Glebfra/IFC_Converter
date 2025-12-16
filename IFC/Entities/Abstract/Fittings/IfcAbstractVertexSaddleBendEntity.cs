using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractVertexSaddleBendEntity : IfcAbstractBendEntity
    {
        public abstract ActionProperty<int> NumSegments { get; }
        public abstract ActionProperty<double> BranchHeight { get; }
        public abstract ActionProperty<double> BranchPipeRadius { get; }
        
        protected IfcAbstractVertexSaddleBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }
        
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }

        protected override IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            XbimVector3D displacement = CalculateDisplacement();
            IfcAxisSettings axisSettings = new IfcAxisSettings(displacement, VectorExtensions.Forward, VectorExtensions.Up);
            representationItems.Add(IfcGeometry.CreateTorus(model, BendRadius, PipeRadius, Angle, NumSegments, axisSettings));
            
            representationItems.Add(IfcGeometry.CreateCylinder(
                model, BranchPipeRadius, BranchHeight, XbimVector3D.Zero, VectorExtensions.Forward.Negated(), VectorExtensions.Right
            ));

            return representationItems;
        }

        protected new XbimVector3D CalculateDisplacement()
        {
            return BendRadius * Math.Tan(Angle / 2) * (VectorExtensions.Forward.Negated() + VectorExtensions.Up.Negated());
        }

        protected XbimVector3D CalculateBranchDisplacement()
        {
            return BendRadius * Math.Tan(Angle / 2) * (VectorExtensions.Forward);
        }

        protected new void ClipPipes()
        {
            IEnumerable<IfcAbstractSegmentEntity> abstractSegmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>();
            
            double clipLength = BendRadius * Math.Tan(Angle / 2);
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in abstractSegmentEntities)
            {
                if (IfcAxis.GetPipeDirectionFromNode(ifcPipeEntity, NodeEntity).IsEqualFixed(VectorExtensions.Forward.Negated()))
                    continue;
                ifcPipeEntity.Clip(NodeEntity, clipLength);
            }
        }
    }
}