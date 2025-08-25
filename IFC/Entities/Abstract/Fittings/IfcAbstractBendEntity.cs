using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Extensions;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometryResource;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;

namespace IFC.Entities.Abstract.Fittings
{
    public abstract class IfcAbstractBendEntity : IfcAbstractFittingEntity
    {
        public abstract double Angle { get; }
        public abstract double BendRadius { get; }
        public abstract double PipeRadius { get; }

        protected IfcAbstractBendEntity(XbimMatrix3D objectMatrix3D) : base(objectMatrix3D) { }
        
        public override IfcProduct CreateAndAdd(IModel model)
        {
            IfcPipeFitting pipeFitting = CreateIfcEntity<IfcPipeFitting>(model);
            ClipPipes();
            return pipeFitting;
        }

        protected abstract IEnumerable<IfcRepresentationItem> CreateShape(IModel model);
        
        protected new T CreateIfcEntity<T>(IModel model)
            where T : IfcPipeFitting, IInstantiableEntity
        {
            T pipeFitting = base.CreateIfcEntity<T>(model);
            pipeFitting.PredefinedType = IfcPipeFittingTypeEnum.BEND;

            IEnumerable<IfcRepresentationItem> representationItems = CreateShape(model);
            AddShapeRepresentation(model, pipeFitting, representationItems);

            return pipeFitting;
        }
        
        protected XbimVector3D CalculateDisplacement()
        {
            XbimMatrix3D My = MatrixExtensions.My(Angle);
            
            XbimVector3D firstDisplacementVector = VectorExtensions.Right;
            XbimVector3D secondDisplacementVector = XbimVector3D.Multiply(firstDisplacementVector, My);
            XbimVector3D displacementVector = (firstDisplacementVector + secondDisplacementVector).Normalized().Negated();

            double displacementLength = BendRadius / Math.Cos(Angle * 0.5);
            return displacementLength * displacementVector;
        }

        protected void ClipPipes()
        {
            IEnumerable<IfcAbstractSegmentEntity> abstractSegmentEntities = ConnectedEntities.OfType<IfcAbstractSegmentEntity>();

            double clipLength = BendRadius * Math.Tan(Angle / 2);
            foreach (IfcAbstractSegmentEntity ifcPipeEntity in abstractSegmentEntities)
            {
                ifcPipeEntity.Clip(NodeEntity, clipLength);
            }
        }
    }
}