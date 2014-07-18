namespace DataParser.Hand
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a finger of the hand.
    /// </summary>
    public class Finger
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Finger"/> class.
        /// </summary>
        public Finger(IList<double> values, double[] palmCenter, double[] palmNormal)
        {
            this.Direction = new[] { values[0], values[1], values[2] };
            this.Length = values[3];
            this.TipPosition = new[] { values[4], values[5], values[6] };
            this.TipVelocity = new[] { values[7], values[8], values[9] };
            this.DipPosition = new[] { values[10], values[11], values[12] };
            this.PipPosition = new[] { values[13], values[14], values[15] };
            this.McpPosition = new[] { values[16], values[17], values[18] };

            // Get the point on the finger that is farthest form the palm center.
            // This distance is the "extended distance" from the palm.
            this.DistanceFromPalmCenter = (new[]
            {
                VectorCalculator.EuclideanDistance(this.TipPosition, palmCenter), 
                VectorCalculator.EuclideanDistance(this.DipPosition, palmCenter), 
                VectorCalculator.EuclideanDistance(this.PipPosition, palmCenter), 
                VectorCalculator.EuclideanDistance(this.McpPosition, palmCenter)
            }).Max();

            // Calculate the finger segments projected to palm normal
            this.DipTipProjection =
                VectorCalculator.Projection(VectorCalculator.VectorDifference(this.DipPosition, this.TipPosition), palmNormal);
            this.PipDipProjection =
                VectorCalculator.Projection(VectorCalculator.VectorDifference(this.PipPosition, this.DipPosition), palmNormal);
            this.McpPipProjection =
                VectorCalculator.Projection(VectorCalculator.VectorDifference(this.McpPosition, this.PipPosition), palmNormal);

            this.Angle = VectorCalculator.AngleOfVector(this.Direction);
        }

        /// <summary>
        /// Gets the direction.
        /// </summary>
        public double[] Direction { get; private set; }

        /// <summary>
        /// Gets the length.
        /// </summary>
        public double Length { get; private set; }

        /// <summary>
        /// Gets the tip position.
        /// </summary>
        public double[] TipPosition { get; private set; }

        /// <summary>
        /// Gets the tip velocity.
        /// </summary>
        public double[] TipVelocity { get; private set; }

        /// <summary>
        /// Gets the dip position.
        /// </summary>
        public double[] DipPosition { get; private set; }

        /// <summary>
        /// Gets the pip position.
        /// </summary>
        public double[] PipPosition { get; private set; }

        /// <summary>
        /// Gets the metacarpal position.
        /// </summary>
        public double[] McpPosition { get; private set; }

        /// <summary>
        /// Gets or sets the distance from palm center.
        /// </summary>
        public double DistanceFromPalmCenter { get; set; }
        
        /// <summary>
        /// Projection of dip to tip finger segment onto palm normal.
        /// </summary>
        public double DipTipProjection { get; private set; }

        /// <summary>
        /// Projection of pip to dip finger segment onto palm normal.
        /// </summary>
        public double PipDipProjection { get; private set; }

        /// <summary>
        /// Projection of metacarpal to pip finger segment onto palm normal.
        /// </summary>
        public double McpPipProjection { get; private set; }

        /// <summary>
        /// Order of the finger along the x-z plane with respect to other fingers.
        /// </summary>
        public double Order { get; set; }

        /// <summary>
        /// Has the angle between the direction vector and the 
        /// </summary>
        public double Angle { get; set; }
    }
}
