using System;
using System.Collections.Generic;
using System.Linq;
using IFC.Entities;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFCtoSTART.Extensions.Entities;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;

namespace IFCtoSTART.Importers
{
    internal class StandardImporter : IImporter
    {
        public virtual IfcPipeSegment[] GetPipeSegments(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeSegment>()
                .ToArray();
        }

        public virtual IfcPipeFitting[] GetBends(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.BEND)
                .ToArray();
        }

        public virtual IfcPipeFitting[] GetTees(IfcProduct[] products)
        {
            return products
                .OfType<IfcPipeFitting>()
                .Where(fitting => fitting.PredefinedType == IfcPipeFittingTypeEnum.JUNCTION)
                .ToArray();
        }

        public virtual IfcPipeSegmentEntity[] CreatePipeSegments(IfcPipeSegment[] pipeSegments)
        {
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipeSegments.Length];

            for (int i = 0; i < pipeSegments.Length; i++)
            {
                IfcPipeSegment pipeSegment = pipeSegments[i];
                IfcLabel name = pipeSegment.Name ?? new IfcLabel("");
                IfcIdentifier tag = pipeSegment.Tag ?? new IfcIdentifier("");
                XbimMatrix3D objectMatrix3D = pipeSegment.ObjectPlacement.ToObjectMatrix3D();

                IPropertySet[] propertySets = pipeSegment.GetPropertySets().ToArray();
                double length = GetPipeLength(propertySets);
                double diameter = GetPipeDiameter(propertySets);

                pipeSegmentEntities[i] = new IfcPipeSegmentEntity(name, tag, objectMatrix3D, length, diameter);
            }

            return pipeSegmentEntities;
        }

        public virtual IfcCadBendEntity[] CreateBends(IfcPipeFitting[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            IfcCadBendEntity[] bendEntities = new IfcCadBendEntity[bends.Length];
            for (int i = 0; i < bends.Length; i++)
            {
                IfcPipeFitting bend = bends[i];
                IfcLabel name = bend.Name ?? new IfcLabel("");
                IfcIdentifier tag = bend.Tag ?? new IfcIdentifier("");
                
                XbimMatrix3D objectMatrix3D = bend.ObjectPlacement.ToObjectMatrix3D();
                XbimVector3D coordinates = objectMatrix3D.Translation;

                IfcAbstractSegmentEntity[] fittingSegments = abstractSegmentEntities
                    .OrderBy(segment => segment.ObjectMatrix3D.Value.Translation.GetDistance(coordinates))
                    .Take(2)
                    .ToArray();
                IfcNodeEntity nodeEntity = new IfcNodeEntity(objectMatrix3D);

                IPropertySet[] propertySets = bend.GetPropertySets().ToArray();
                double angle = GetBendAngle(propertySets);
                double bendRadius = GetBendRadius(propertySets);
                double length = angle * bendRadius;
                double pipeDiameter = fittingSegments.Max(segment => segment.Diameter);
                
                double clipLength = bendRadius * Math.Tan(angle / 2);
                foreach (IfcAbstractSegmentEntity ifcAbstractSegmentEntity in fittingSegments)
                {
                    ifcAbstractSegmentEntity.Clip(nodeEntity, -clipLength);
                }

                IfcCadBendEntity cadBendEntity = new IfcCadBendEntity(
                    name,
                    tag,
                    objectMatrix3D,
                    length,
                    angle,
                    bendRadius,
                    pipeDiameter / 2
                );
                cadBendEntity.ConnectedEntities.AddRange(fittingSegments);
                cadBendEntity.PropertySets.AddRange(propertySets);
                
                bendEntities[i] = cadBendEntity;
            }

            return bendEntities;
        }

        public virtual IfcWeldedTeeEntity[] CreateWeldedTees(IfcPipeFitting[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            throw new System.NotImplementedException();
        }
        
        protected static double GetBendAngle(IEnumerable<IPropertySet> propertySets)
        {
            Pset_PipeFittingTypeBend psetPipeFittingTypeBend = propertySets.OfType<Pset_PipeFittingTypeBend>().First();
            return psetPipeFittingTypeBend.BendAngle;
        }

        protected static double GetBendRadius(IEnumerable<IPropertySet> propertySets)
        {
            Pset_PipeFittingTypeBend psetPipeFittingTypeBend = propertySets.OfType<Pset_PipeFittingTypeBend>().First();
            return psetPipeFittingTypeBend.BendRadius;
        }

        protected static double GetPipeLength(IEnumerable<IPropertySet> propertySets)
        {
            Qto_PipeSegmentBaseQuantities qtoPipeSegmentBaseQuantities = propertySets.OfType<Qto_PipeSegmentBaseQuantities>().First();
            return qtoPipeSegmentBaseQuantities.Length;
        }

        protected static double GetPipeDiameter(IEnumerable<IPropertySet> propertySets)
        {
            Pset_PipeSegmentTypeCommon psetPipeSegmentTypeCommon = propertySets.OfType<Pset_PipeSegmentTypeCommon>().First();
            return psetPipeSegmentTypeCommon.OuterDiameter;
        }
    }
}