using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using Ifc.Builders;
using Ifc.Builders.Properties;
using Ifc.Interfaces;
using IFCConverter.Interfaces;
using MathNet.Numerics.LinearAlgebra;
using Start.Attributes;
using Start.Interfaces;
using Utils;
using Xbim.Common;
using Xbim.Ifc.Extensions;
using Xbim.Ifc4.Interfaces;
using Xbim.Ifc4.MeasureResource;
using Xbim.Ifc4.PropertyResource;

namespace IFCConverter.Converters.Elements
{
    internal abstract class IfcElementConverter<TStart, TIfc> : IIfcElementConverter
        where TStart : IStartEntity
        where TIfc : IIfcProduct, IInstantiableEntity
    {
        private readonly Logger _logger = Logger.GetInstance();
        protected readonly IModel _Model;

        protected IfcElementConverter(IModel model)
        {
            _Model = model;
        }

        public object BuildIfc(object start)
        {
            return BuildIfcElement((TStart)start);
        }

        public object BuildStart(object ifc)
        {
            return BuildStartElement((TIfc)ifc)!;
        }

        [Pure]
        public abstract IIfcGeometry CreateGeometry(TStart start);

        [Pure]
        public abstract Matrix<double> CreateObjectMatrix(TStart start);

        [Pure]
        public abstract IIfcProductBuilder<TIfc> CreateBuilder(TStart start);

        public TIfc BuildIfcElement(TStart start)
        {
            Matrix<double> objectMatrix = CreateObjectMatrix(start);

            IIfcGeometry geometry = CreateGeometry(start);
            _logger.Info($"Created geometry {geometry.GetType().FullName}");

            IIfcProductBuilder<TIfc> builder = CreateBuilder(start);
            TryAddMaterial(start, builder);

            builder.AssignGeometry(geometry);
            builder.CreateObjectPlacement(_Model, objectMatrix);

            IIfcPropertySet psetStart = CreateStartPropertySet(start);
            builder.PropertySets.Add(psetStart);

            return builder.CreateInstance(_Model);
        }

        public abstract TStart BuildStartElement(TIfc ifc);

        protected string GenerateTag(TStart start)
        {
            return start.GetType().GetCustomAttribute<StartElementAttribute>().Type.ToString();
        }

        protected string GenerateName(TStart start)
        {
            return !start.Name.IsEmpty()
                ? start.Name
                : start switch
                {
                    IStartTwoNodeEntity twoNodeEntity =>
                        $"{start.GetType().Name}_{twoNodeEntity.StartNode.Name}_{twoNodeEntity.EndNode.Name}",
                    IStartOneNodeEntity oneNodeEntity => $"{start.GetType().Name}_{oneNodeEntity.Node.Name}",
                    _ => $"{start.GetType().Name}_{start.ID}"
                };
        }

        private void TryAddMaterial(TStart start, IIfcProductBuilder<TIfc> builder)
        {
            if (start is not IStartMaterializedEntity materializedEntity)
                return;

            IIfcMaterialBuilder materialBuilder = new IfcMaterialBuilder(materializedEntity.MaterialName, "", "");
            if (materialBuilder.GetOrCreateMaterial(_Model, out IIfcMaterial material))
                _logger.Info($"Created material with name: {material.Name}");
            builder.AssignMaterial(material);
        }

        private IIfcPropertySet CreateStartPropertySet(TStart start)
        {
            IDictionary<string, string> psetData = start.GetData();
            IEnumerable<IIfcPropertySingleValueBuilder<IIfcPropertySingleValue>> propertyBuilders = psetData
                .Select(pair =>
                {
                    string propertyName = pair.Key;
                    IfcText propertyValue = new(pair.Value);
                    string propertyDescription = "";
                    return new IfcPropertySingleValueBuilder<IfcPropertySingleValue>(propertyName, propertyDescription,
                        propertyValue, null!);
                });
            IIfcPropertySetBuilder propertySetBuilder = new IfcPropertySetBuilder("Pset_Start", propertyBuilders);
            return propertySetBuilder.CreatePropertySet(_Model);
        }
    }
}