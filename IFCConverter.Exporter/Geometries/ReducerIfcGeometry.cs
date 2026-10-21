using System;
using System.Collections.Generic;
using System.Linq;
using IFCConverter.Domain.Entities;
using IFCConverter.Geometry.Mesh.Builders;
using IFCConverter.IFC.Geometries;
using IFCConverter.IFC.Interfaces;
using Xbim.Common;
using Xbim.Ifc4.Interfaces;

namespace IFCConverter.Exporter.Geometries
{
    internal sealed class ReducerIfcGeometry : IfcGeometry
    {
        public ReducerIfcGeometry(IIfcBuilder geometryBuilder, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilder, representationContext)
        {
        }

        public ReducerIfcGeometry(IEnumerable<IIfcBuilder> geometryBuilders, IIfcRepresentationContext representationContext = null) 
            : base(geometryBuilders, representationContext)
        {
        }

        public static ReducerIfcGeometry Create(IModel model, Reducer reducer)
        {
            double minRadius = reducer.Ports.Min(port => port.Metadata.Diameter);
            double maxRadius = reducer.Ports.Max(port => port.Metadata.Diameter);

            throw new NotImplementedException();
        }
    }
}