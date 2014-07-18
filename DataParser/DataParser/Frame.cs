namespace DataParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using global::DataParser.Hand;

    /// <summary>
    /// A frame is a snap shot in time of the palm and fingers of the hand.
    /// </summary>
    public class Frame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class.
        /// </summary>
        /// <param name="frameData">
        /// The frame data.
        /// </param>
        /// <param name="sign">
        /// The sign.
        /// </param>
        public Frame(string frameData, char sign)
        {
            this.Sign = sign;

            string[] splits = frameData.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);

            // Should have 11 items

            // Create palm
            this.Palm = new Palm(DataParser.ParseDataString(splits[0]));

            // Create fingers
            for (int i = 1; i <= 10; i = i + 2)
            {
                this.Fingers.Add(
                    new Finger(
                        DataParser.ParseDataString(splits[i] + "," + splits[i + 1]),
                        this.Palm.PalmPosition,
                        this.Palm.PalmNormal));
            }

            this.OrderFingers();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Frame"/> class. 
        /// Creates an empty frame.
        /// </summary>
        public Frame()
        {
        }

        /// <summary>
        /// All fingers for the 
        /// </summary>
        public List<Finger> Fingers { get; set; }

        /// <summary>
        /// Palm object for the frame.
        /// </summary>
        public Palm Palm { get; set; }

        /// <summary>
        /// Correct Sign for the frame
        /// </summary>
        public char Sign { get; set; }

        /// <summary>
        /// This is the total distance traveled by the hand across the entire data frame.
        /// </summary>
        public double AverageDistance { get; set; }

        /// <summary>
        /// Gets or sets the average spread.
        /// </summary>
        public double AverageSpread { get; set; }

        /// <summary>
        /// Gets or sets the average triangular spread.
        /// </summary>
        public double AverageTriangularSpread { get; set; }

        /// <summary>
        /// Gets the spread. The distance between fingers.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetSpread()
        {
            // Calculate finger distances
            double spread = 0;
            for (int i = 0; i < 3; i++)
            {
                spread += VectorCalculator.EuclideanDistance(this.Fingers[i].TipPosition, this.Fingers[i + 1].TipPosition)
                        + VectorCalculator.EuclideanDistance(this.Fingers[i].DipPosition, this.Fingers[i + 1].DipPosition)
                        + VectorCalculator.EuclideanDistance(this.Fingers[i].McpPosition, this.Fingers[i + 1].McpPosition)
                        + VectorCalculator.EuclideanDistance(this.Fingers[i].PipPosition, this.Fingers[i + 1].PipPosition);
            }

            return spread;
        }

        /// <summary>
        /// Calculates and returns the triangular spread
        /// The distance between finger tips and their metacarpal midpoints.
        /// </summary>
        /// <returns>
        /// The <see cref="double"/>.
        /// </returns>
        public double GetTriangularSpread()
        {
            double spread = 0.0;

            // Calculate finger distances
            for (int i = 0; i < 3; i++)
            {
                // Get the point between the two meta
                double[] middleMcp =
                {
                    (this.Fingers[i].McpPosition[0] + this.Fingers[i + i].McpPosition[0]) / 2,
                    (this.Fingers[i].McpPosition[1] + this.Fingers[i + i].McpPosition[1]) / 2,
                    (this.Fingers[i].McpPosition[2] + this.Fingers[i + i].McpPosition[2]) / 2
                };

                double a = VectorCalculator.EuclideanDistance(this.Fingers[i].TipPosition, this.Fingers[i + 1].TipPosition);
                double b = VectorCalculator.EuclideanDistance(this.Fingers[i].TipPosition, middleMcp);
                double c = VectorCalculator.EuclideanDistance(this.Fingers[i + 1].TipPosition, middleMcp);

                double s = (a + b + c) / 2;

                // Heron's Formula
                double area = Math.Sqrt(s * (s - a) * (s - b) * (s - c));

                if (!double.IsNaN(area))
                {
                    spread += area;
                }
            }

            return spread;
        }

        /// <summary>
        /// Calculates the order of the fingers along the x-axis.
        /// If two fingers have the same x-coordinate (this actually happens sometimes) then they are given the same order.
        /// </summary>
        public void OrderFingers()
        {
            List<KeyValuePair<Finger, Finger>> identicalFingers = new List<KeyValuePair<Finger, Finger>>();

            // Fingers sorted by least x-coordinate, ascending
            SortedList<double, Finger> sortedFingers = new SortedList<double, Finger>();

            foreach (Finger finger in this.Fingers)
            {
                if (!sortedFingers.ContainsKey(finger.PipPosition[0]))
                {
                    sortedFingers.Add(finger.PipPosition[0], finger);
                }
                else
                {
                    identicalFingers.Add(new KeyValuePair<Finger, Finger>(sortedFingers[finger.PipPosition[0]], finger));
                }
            }

            int j = 1;
            foreach (KeyValuePair<double, Finger> pair in sortedFingers)
            {
                pair.Value.Order = j;

                // Use local variable to avoid a closure
                KeyValuePair<double, Finger> currentPair = pair;

                // Check if there was another finger with the same x-coordinate
                foreach (
                    Finger sameFinger in
                        identicalFingers.Where(kvp => kvp.Key.Equals(currentPair.Value)).Select(kvp => kvp.Value))
                {
                    sameFinger.Order = j;
                }

                j++;
            }

            sortedFingers.Clear();
            identicalFingers.Clear();
        }
    }
}
