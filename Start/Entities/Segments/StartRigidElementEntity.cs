using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Augmenters;
using Start.Interfaces;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents a rigid element entity in the Start framework.
    ///     Inherits from <see cref="StartAbstractSegmentEntity" /> and implements the
    ///     <see cref="IStartMaterializedEntity" /> interface.
    /// </summary>
    [StartElement(StartElementTypeEnum.RIGID_ELEMENT, typeof(StartRigidElementDiameterAugmenter))]
    public sealed class StartRigidElementEntity : StartAbstractSegmentEntity, IStartMaterializedEntity
    {
        /// <summary>
        ///     Gets or sets the name of the material associated with the rigid element.
        /// </summary>
        [JsonProperty(StartPropertyName.MaterialName)]
        public string MaterialName { get; set; } = string.Empty;
    }
}