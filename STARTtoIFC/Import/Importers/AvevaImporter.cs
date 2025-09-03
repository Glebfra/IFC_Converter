using System;
using System.Linq;
using IFC.Entities.Abstract.Segments;
using IFC.Entities.Fittings.CAD;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using STARTtoIFC.Extensions.Entities;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.GeometricModelResource;
using Xbim.Ifc4.Kernel;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.ProductExtension;
using Xbim.Ifc4.ProfileResource;
using Xbim.Ifc4.RepresentationResource;

namespace STARTtoIFC.Importers
{
    internal class AvevaImporter : StandardImporter
    {
        public AvevaImporter(IModel model) : base(model) { }
        
        public override IfcElement[] GetPipeSegments(IfcProduct[] products)
        {
            return GetElementByType(products, "TUBING");
        }

        // public override IfcElement[] GetBends(IfcProduct[] products)
        // {
        //     return GetElementByType(products, "ELBOW");
        // }
        //
        // public override IfcElement[] GetTees(IfcProduct[] products)
        // {
        //     return GetElementByType(products, "TEE");
        // }

        public override IfcPipeSegmentEntity[] CreatePipeSegments(IfcElement[] pipes)
        {
            IfcPipeSegmentEntity[] pipeSegmentEntities = new IfcPipeSegmentEntity[pipes.Length];
            for (int i = 0; i < pipes.Length; i++)
            {
                IfcElement pipe = pipes[i];
                IfcLabel name = pipe.Name ?? new IfcLabel("");
                IfcIdentifier tag = pipe.Tag ?? new IfcIdentifier("");
                
                IfcExtrudedAreaSolid? extrudedAreaSolid = null;
                foreach (IfcRepresentation ifcRepresentation in pipe.Representation.Representations)
                {
                    extrudedAreaSolid = ifcRepresentation.Items.OfType<IfcExtrudedAreaSolid>().FirstOrDefault();
                    if (extrudedAreaSolid != null)
                        break;
                }
                if (extrudedAreaSolid == null)
                    throw new NullReferenceException("Pipe does not have IfcExtrudedAreaSolid representation.");
                
                XbimMatrix3D shapeMatrix3D = extrudedAreaSolid.Position.ToObjectMatrix3D().RescaleTranslation(_LengthUnit.Power);
                double length = GetPipeLength(extrudedAreaSolid) * _LengthUnit.Power;
                double diameter = GetPipeDiameter(extrudedAreaSolid) * _LengthUnit.Power;
                
                IPropertySet[] propertySets = pipe.GetPropertySets().ToArray();

                IfcPipeSegmentEntity pipeSegmentEntity = new IfcPipeSegmentEntity(name, tag, shapeMatrix3D, length, diameter);
                pipeSegmentEntity.PropertySets.AddRange(propertySets);
                pipeSegmentEntities[i] = pipeSegmentEntity;
            }

            return pipeSegmentEntities;
        }

        /*public override IfcCadBendEntity[] CreateBends(IfcElement[] bends, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            IfcCadBendEntity[] bendEntities = new IfcCadBendEntity[bends.Length];
            for (int i = 0; i < bends.Length; i++)
            {
                IfcElement bend = bends[i];
                IfcLabel name = bend.Name ?? new IfcLabel("");
                IfcIdentifier tag = bend.Tag ?? new IfcIdentifier("");
                
                IPropertySet[] propertySets = bend.GetPropertySets().ToArray();
                
                AVEVA_EntityParameters? avevaEntityParameters = propertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                if (avevaEntityParameters == null)
                    throw new Exception("Bend does not have AVEVA_EntityParameters property set.");
                
                AVEVA_Pset? avevaPset = propertySets.OfType<AVEVA_Pset>().FirstOrDefault();
                if (avevaPset == null)
                    throw new Exception("Bend does not have AVEVA_Pset property set.");
            }

            return bendEntities;
        }*/

        public override IfcWeldedTeeEntity[] CreateWeldedTees(IfcElement[] tees, IfcAbstractSegmentEntity[] abstractSegmentEntities)
        {
            IfcWeldedTeeEntity[] weldedTeeEntities = Array.Empty<IfcWeldedTeeEntity>();
            return weldedTeeEntities;
        }

        private IfcElement[] GetElementByType(IfcProduct[] products, IfcText type)
        {
            return products
                .Select(product => new { Product = product, PropertySets = product.GetPropertySets() })
                .Where(item =>
                {
                    AVEVA_EntityParameters? pset = item.PropertySets.OfType<AVEVA_EntityParameters>().FirstOrDefault();
                    return pset?.E3DType.Value == type;
                })
                .Select(item => item.Product)
                .Cast<IfcElement>()
                .ToArray();
        }

        private double GetPipeLength(IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            return extrudedAreaSolid.Depth;
        }

        private double GetPipeDiameter(IfcExtrudedAreaSolid extrudedAreaSolid)
        {
            IfcCircleProfileDef? circleProfileDef = extrudedAreaSolid.SweptArea as IfcCircleProfileDef;
            if (circleProfileDef == null)
                throw new Exception("Pipe profile is not a circle.");
            return circleProfileDef.Radius * 2;
        }
    }
}