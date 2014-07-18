namespace DataParser.Hand
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents and constructs a palm from the LEAP motion.
    /// </summary>
    public class Palm
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Palm"/> class.
        /// </summary>
        /// <param name="values">
        /// The values.
        /// </param>
        public Palm(IList<double> values)
        {
            this.PalmNormal = new[] { values[0], values[1], values[2] };
            this.PalmPosition = new[] { values[3], values[4], values[5] };
            this.PalmVelocity = new[] { values[6], values[7], values[8] };
            this.SphereCenter = new[] { values[9], values[10], values[11] };

            this.Confidence = values[12];
            this.PinchStrength = values[13];
            this.GrabStrength = values[14];
            this.SphereRadius = values[15];

            this.Angle = VectorCalculator.AngleOfVector(this.PalmNormal);
        }

        /// <summary>
        /// Gets the palm normal.
        /// </summary>
        public double[] PalmNormal { get; private set; }

        /// <summary>
        /// Gets the palm position.
        /// </summary>
        public double[] PalmPosition { get; private set; }

        /// <summary>
        /// Gets the palm velocity.
        /// </summary>
        public double[] PalmVelocity { get; private set; }

        /// <summary>
        /// Gets the sphere center.
        /// </summary>
        public double[] SphereCenter { get; private set; }

        /// <summary>
        /// Gets the pinch strength.
        /// </summary>
        public double PinchStrength { get; private set; }

        /// <summary>
        /// Gets the grab strength.
        /// </summary>
        public double GrabStrength { get; private set; }

        /// <summary>
        /// Gets the confidence.
        /// </summary>
        public double Confidence { get; private set; }

        /// <summary>
        /// Gets the sphere radius.
        /// </summary>
        public double SphereRadius { get; private set; }

        /// <summary>
        /// Gets or sets the angle.
        /// </summary>
        public double Angle { get; set; }
    }
}
