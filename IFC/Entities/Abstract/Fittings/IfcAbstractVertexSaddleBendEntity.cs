using System;
using System.Collections.Generic;
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

        private IEnumerable<IfcRepresentationItem> CreateShape(IModel model)
        {
            List<IfcRepresentationItem> representationItems = new List<IfcRepresentationItem>();
            IfcAxisSettings axisSettings = new IfcAxisSettings(XbimVector3D.Zero, VectorExtensions.X, VectorExtensions.Y);
            representationItems.Add(IfcVertexGeometry.CreateTorus(model, BendRadius, PipeRadius, Angle, NumSegments, axisSettings));
            
            XbimVector3D branchDisplacement = BendRadius * Math.Tan(Angle / 2) * (VectorExtensions.Forward + VectorExtensions.Right);
            representationItems.Add(IfcGeometry.CreateCylinder(
                model, BranchPipeRadius, BranchHeight, branchDisplacement, VectorExtensions.Forward.Negated(), VectorExtensions.Right
            ));

            return representationItems;
        }
    }
}