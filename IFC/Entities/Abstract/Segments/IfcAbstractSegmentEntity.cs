using System.Linq;
using IFC.Entities.Interfaces;
using IFC.Entities.Segments;
using IFC.Extensions;
using IFC.PropertySets;
using IFC.Tools;
using Xbim.Common;
using Xbim.Common.Geometry;
using Xbim.Ifc4.HvacDomain;
using Xbim.Ifc4.Interfaces;

namespace IFC.Entities.Abstract.Segments
{
    /// <summary>
    /// Represents an abstract segment entity in the IFC model.
    /// </summary>
    public abstract class IfcAbstractSegmentEntity : IfcAbstractEntity, IIfcClippable, IIfcTwoNodeEntity
    {
        /// <summary>
        /// Gets the diameter of the segment.
        /// </summary>
        public ActionProperty<double> Diameter { get; private set; }
        
        /// <summary>
        /// Gets the length of the segment.
        /// </summary>
        public ActionProperty<double> Length { get; private set; }
        
        /// <summary>
        /// Gets the node entities associated with the segment.
        /// </summary>
        public IfcNodeEntity[] NodeEntities { get; private set; }

        /// <summary>
        /// Gets the start point of the segment.
        /// </summary>
        public XbimVector3D StartPoint => StartNode.ObjectMatrix3D.Translation;
        
        /// <summary>
        /// Gets the end point of the segment.
        /// </summary>
        public XbimVector3D EndPoint => EndNode.ObjectMatrix3D.Translation;

        /// <summary>
        /// Gets the start node of the segment.
        /// </summary>
        public IfcNodeEntity StartNode
        {
            get => NodeEntities[0];
            set => NodeEntities[0] = value;
        }

        /// <summary>
        /// Gets the end node of the segment.
        /// </summary>
        public IfcNodeEntity EndNode
        {
            get => NodeEntities[1];
            set => NodeEntities[1] = value;
        }

        /// <summary>
        /// Gets the direction vector of the segment.
        /// </summary>
        public XbimVector3D SegmentDirection => ObjectMatrix3D.Value.Forward * Length;

        private readonly XbimVector3D _pipeDirection;
        private readonly XbimVector3D _fakeDirection;

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractSegmentEntity"/> class with the specified matrix, length, and diameter.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the segment.</param>
        /// <param name="length">The length of the segment.</param>
        /// <param name="diameter">The diameter of the segment.</param>
        protected IfcAbstractSegmentEntity(XbimMatrix3D matrix3D, double length, double diameter) 
            : base(matrix3D)
        {
            XbimMatrix3D secondMatrix3D = XbimMatrix3D.CreateWorld(
                matrix3D.Translation + matrix3D.Forward * length, 
                matrix3D.Forward, 
                matrix3D.Up
            );
            NodeEntities = new IfcNodeEntity[]
            {
                new IfcNodeEntity(matrix3D),
                new IfcNodeEntity(secondMatrix3D),
            };
            Length = length;
            Diameter = diameter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IfcAbstractSegmentEntity"/> class with the specified matrix, length, diameter, and node entities.
        /// </summary>
        /// <param name="matrix3D">The transformation matrix of the segment.</param>
        /// <param name="length">The length of the segment.</param>
        /// <param name="diameter">The diameter of the segment.</param>
        /// <param name="nodeEntities">The node entities associated with the segment.</param>
        protected IfcAbstractSegmentEntity(XbimMatrix3D matrix3D, double length, double diameter, IfcNodeEntity[] nodeEntities)
            : base(matrix3D)
        {
            NodeEntities = nodeEntities;
            Length = length;
            Diameter = diameter;

            _pipeDirection = matrix3D.Forward * Length;
            _fakeDirection = EndNode.ObjectMatrix3D.Translation - StartNode.ObjectMatrix3D.Translation;
        }
        
        /// <summary>
        /// Divides the segment to the 2 separate pipe segments <see cref="IfcPipeSegmentEntity"/>. 
        /// </summary>
        /// <param name="divisionRate">The division rate from 0 to 1, where 0 - <see cref="StartPoint"/>, 1 - <see cref="EndPoint"/></param>
        public IfcPipeSegmentEntity DividePipe(double divisionRate)
        {
            XbimVector3D dividePoint = VectorExtensions.Lerp(StartPoint, EndPoint, divisionRate);
            XbimMatrix3D divideMatrix = MatrixExtensions.CreateWorld(dividePoint, EndPoint - StartPoint);
            
            IfcNodeEntity divideNode = new IfcNodeEntity(divideMatrix);
            IfcNodeEntity endNode = EndNode;

            double firstLength = Length * divisionRate;
            double secondLength = Length - firstLength;

            Length = firstLength;
            NodeEntities[1] = divideNode;
            IfcPipeSegmentEntity secondSegment = new IfcPipeSegmentEntity(Name + "_Auto_Divide", Tag, divideMatrix, secondLength, Diameter, new IfcNodeEntity[] { divideNode, endNode });

            return secondSegment;
        }

        /// <summary>
        /// Clips the segment at the specified node entity by the given clip length.
        /// </summary>
        /// <param name="nodeEntity">The node entity at which to clip the segment.</param>
        /// <param name="clipLength">The length to clip from the segment.</param>
        public void Clip(IfcNodeEntity nodeEntity, double clipLength)
        {
            if (IsStartNode(nodeEntity))
                MovePipe(ObjectMatrix3D.Value.Forward * clipLength);
            Length.Value -= clipLength;
        }

        /// <summary>
        /// Moves the segment by the specified displacement vector.
        /// </summary>
        /// <param name="displacement">The displacement vector to move the segment.</param>
        public void MovePipe(XbimVector3D displacement)
        {
            ObjectMatrix3D.Value = XbimMatrix3D.CreateWorld(
                ObjectMatrix3D.Value.Translation + displacement,
                ObjectMatrix3D.Value.Forward,
                ObjectMatrix3D.Value.Up
            );
        }

        /// <summary>
        /// Gets the fake displacement vector for the specified node entity.
        /// </summary>
        /// <param name="nodeEntity">The node entity for which to calculate the displacement vector.</param>
        /// <returns>The fake displacement vector.</returns>
        public XbimVector3D GetFakeDisplacementVector(IfcNodeEntity nodeEntity)
        {
            return GetFakeDisplacementVector(nodeEntity.ObjectMatrix3D.Translation);
        }

        /// <summary>
        /// Gets the fake displacement vector for the specified node entity.
        /// </summary>
        /// <param name="coordinates">The coordinates for which to calculate the displacement vector.</param>
        /// <returns>The fake displacement vector.</returns>
        public XbimVector3D GetFakeDisplacementVector(XbimVector3D coordinates)
        {
            XbimVector3D startCoordinates = ObjectMatrix3D.Value.Translation;
            XbimVector3D endCoordinates = startCoordinates + ObjectMatrix3D.Value.Forward * Length;
            bool isNegated = (coordinates - startCoordinates).Length <
                             (coordinates - endCoordinates).Length;
            XbimVector3D displacement = _fakeDirection - _pipeDirection;
            return isNegated ? displacement : displacement.Negated();
        }

        /// <summary>
        /// Performs operations before creating the IFC entity.
        /// </summary>
        protected override void PreCreate()
        {
            base.PreCreate();

            Pset_PipeSegmentTypeCommon? psetPipeSegmentTypeCommon = PropertySets.OfType<Pset_PipeSegmentTypeCommon>().FirstOrDefault();
            if (psetPipeSegmentTypeCommon != null)
            {
                psetPipeSegmentTypeCommon.NominalDiameter.Value = Diameter.Value;
                Diameter.OnValueChange += () => psetPipeSegmentTypeCommon.NominalDiameter.Value = Diameter.Value;
            }

            Qto_PipeSegmentBaseQuantities? qtoPipeSegmentBaseQuantities = PropertySets.OfType<Qto_PipeSegmentBaseQuantities>().FirstOrDefault();
            if (qtoPipeSegmentBaseQuantities != null)
            {
                qtoPipeSegmentBaseQuantities.Length.Value = Length.Value;
                Length.OnValueChange += () => qtoPipeSegmentBaseQuantities.Length.Value = Length.Value;
            }
        }

        /// <summary>
        /// Creates an IFC entity of the specified type and adds it to the model.
        /// </summary>
        /// <typeparam name="T">The type of the IFC entity to create.</typeparam>
        /// <param name="model">The model to which the entity will be added.</param>
        /// <param name="pipeSegmentType">The predefined type of the pipe segment.</param>
        /// <returns>The created IFC entity.</returns>
        protected T CreateIfcEntity<T>(IModel model, IfcPipeSegmentTypeEnum pipeSegmentType)
            where T : IfcPipeSegment, IInstantiableEntity
        {
            T pipeSegment = base.CreateIfcEntity<T>(model);
            pipeSegment.PredefinedType = pipeSegmentType;

            return pipeSegment;
        }

        /// <summary>
        /// Determines whether the specified node entity is the start node of the segment.
        /// </summary>
        /// <param name="nodeEntity">The node entity to check.</param>
        /// <returns><c>true</c> if the node entity is the start node; otherwise, <c>false</c>.</returns>
        private bool IsStartNode(IfcNodeEntity nodeEntity)
        {
            XbimVector3D nodeCoordinates = nodeEntity.ObjectMatrix3D.Translation;
            XbimVector3D startPipeCoordinates = ObjectMatrix3D.Value.Translation;
            XbimVector3D endPipeCoordinates = ObjectMatrix3D.Value.Translation + ObjectMatrix3D.Value.Forward * Length.Value;

            return (nodeCoordinates - startPipeCoordinates).Length < (nodeCoordinates - endPipeCoordinates).Length;
        }
    }
}