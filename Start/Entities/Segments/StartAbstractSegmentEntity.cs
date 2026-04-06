using System.Linq;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Newtonsoft.Json;
using Start.API;
using Start.Attributes;
using Start.Converters;
using Start.Interfaces;
using Start.StartProperties;
using MatrixExtensions = Utils.MatrixExtensions;

namespace Start.Entities.Segments
{
    /// <summary>
    ///     Represents an abstract base class for segment entities in the Start framework.
    ///     Implements various interfaces for clippable, segment, and two-node entities.
    /// </summary>
    public abstract class StartAbstractSegmentEntity : StartAbstractEntity,
        IStartClippableEntity, IStartSegmentEntity, IStartTwoNodeEntity
    {
        /// <summary>
        ///     Gets or sets the wall thickness of the segment.
        /// </summary>
        [JsonProperty(StartPropertyName.WallThickness)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> WallThickness { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the pressure within the segment.
        /// </summary>
        [JsonProperty(StartPropertyName.Pressure)]
        [JsonConverter(typeof(JsonStartConverter<PressureValueProperty<double>>))]
        public IStartValueProperty<double> Pressure { get; set; } = new PressureValueProperty<double>();

        /// <summary>
        ///     Gets or sets the test pressure of the segment.
        /// </summary>
        [JsonProperty(StartPropertyName.TestPressure)]
        [JsonConverter(typeof(JsonStartConverter<PressureValueProperty<double>>))]
        public IStartValueProperty<double> TestPressure { get; set; } = new PressureValueProperty<double>();

        /// <summary>
        ///     Gets or sets the temperature of the segment.
        /// </summary>
        [JsonProperty(StartPropertyName.Temperature)]
        [JsonConverter(typeof(JsonStartConverter<TemperatureValueProperty<double>>))]
        public IStartValueProperty<double> Temperature { get; set; } = new TemperatureValueProperty<double>();

        /// <summary>
        ///     Gets or sets the projection of the segment along the X-axis.
        /// </summary>
        [JsonProperty(StartPropertyName.ProjectionAlongOXAxis)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> ProjectionAlongOXAxis { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the projection of the segment along the Y-axis.
        /// </summary>
        [JsonProperty(StartPropertyName.ProjectionAlongOYAxis)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> ProjectionAlongOYAxis { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the projection of the segment along the Z-axis.
        /// </summary>
        [JsonProperty(StartPropertyName.ProjectionAlongOZAxis)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public IStartValueProperty<double> ProjectionAlongOZAxis { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets or sets the unit weight of the pipe.
        /// </summary>
        [JsonProperty(StartPropertyName.Weight)]
        [JsonConverter(typeof(JsonStartConverter<MassValueProperty<double>>))]
        public IStartValueProperty<double> PipeUnitWeight { get; set; } = new MassValueProperty<double>();

        /// <summary>
        ///     Gets or sets the name of the segment.
        /// </summary>
        [JsonProperty(StartPropertyName.PipeName)]
        public override string Name { get; set; } = string.Empty;

        /// <summary>
        ///     Clips the segment by adjusting its start position and projection vector based on the specified position and length.
        /// </summary>
        /// <param name="position">The position to clip from.</param>
        /// <param name="length">The length to clip.</param>
        public void Clip(Vector<double> position, double length)
        {
            if (IsStartPosition(position))
                StartPosition += Projection.Normalize(2) * length;
            Projection -= Projection.Normalize(2) * length;
        }

        /// <summary>
        ///     Gets or sets the diameter of the segment.
        /// </summary>
        [JsonProperty(StartPropertyName.Diameter)]
        [JsonConverter(typeof(JsonStartConverter<LengthValueProperty<double>>))]
        public virtual IStartValueProperty<double> Diameter { get; set; } = new LengthValueProperty<double>();

        /// <summary>
        ///     Gets the length of the segment, calculated as the 2-norm of the projection vector.
        /// </summary>
        [JsonIgnore]
        public virtual double Length => Projection.Norm(2);

        /// <summary>
        ///     Gets or sets the start position of the segment as a vector.
        /// </summary>
        [JsonIgnore]
        public Vector<double> StartPosition { get; set; } = default!;

        /// <summary>
        ///     Gets or sets the projection vector of the segment.
        /// </summary>
        [JsonIgnore]
        public virtual Vector<double> Projection
        {
            get => new DenseVector(new[]
            {
                ProjectionAlongOXAxis.StartProperty,
                ProjectionAlongOYAxis.StartProperty,
                ProjectionAlongOZAxis.StartProperty
            });
            set
            {
                ProjectionAlongOXAxis.CreateFromStart(value[0]);
                ProjectionAlongOYAxis.CreateFromStart(value[1]);
                ProjectionAlongOZAxis.CreateFromStart(value[2]);
            }
        }

        /// <summary>
        ///     Gets the end position of the segment, calculated as the sum of the start position and projection vector.
        /// </summary>
        [JsonIgnore]
        public Vector<double> EndPosition => StartPosition + Projection;

        /// <summary>
        ///     Gets the start node entity of the segment.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public IStartNodeEntity StartNode => ConnectedEntities.OfType<IStartNodeEntity>().ElementAt(0);

        /// <summary>
        ///     Gets the end node entity of the segment.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public IStartNodeEntity EndNode => ConnectedEntities.OfType<IStartNodeEntity>().ElementAt(1);

        /// <summary>
        ///     Gets the start transformation matrix of the segment.
        /// </summary>
        [JsonIgnore]
        [StartIgnore]
        public virtual Matrix<double> TransformationMatrix =>
            MatrixExtensions.CreateTransitionWithWorldUp(StartPosition, Projection);

        /// <summary>
        ///     Determines whether the specified position is closer to the start position of the segment.
        /// </summary>
        /// <param name="position">The position to check.</param>
        /// <returns><c>true</c> if the position is closer to the start position; otherwise, <c>false</c>.</returns>
        public bool IsStartPosition(Vector<double> position)
        {
            Vector<double> startDirection = StartPosition - position;
            Vector<double> endDirection = EndPosition - position;
            return startDirection.L2Norm() < endDirection.L2Norm();
        }
    }
}