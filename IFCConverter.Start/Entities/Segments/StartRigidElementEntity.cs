using IFCConverter.Start.API;
using IFCConverter.Start.Attributes;
using IFCConverter.Start.Augmenters;
using IFCConverter.Start.Interfaces;
using Newtonsoft.Json;

namespace IFCConverter.Start.Entities.Segments
{
    /// <summary>
    ///     Represents a rigid element entity in the IFCConverter.Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentEntity" /> and implements the
    ///     <see cref="IStartMaterializedEntity" /> interface.
    /// </summary>
    [StartElement(StartElementTypeEnum.RIGID_ELEMENT, typeof(StartSegmentEntityDiameterAugmenter), typeof(StartClippableEntityAugmenter))]
    public sealed class StartRigidElementEntity : StartAbstractSegmentEntity, IStartMaterializedEntity
    {
        /// <summary>
        ///     Gets or sets the name of the material associated with the rigid element.
        /// </summary>
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
    }
}