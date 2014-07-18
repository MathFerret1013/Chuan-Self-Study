namespace DataParser
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    /// <summary>
    /// The vector calculator.
    /// </summary>
    public static class VectorCalculator
    {
        /// <summary>
        /// Calculates the Euclidean distance between two vectors.
        /// </summary>
        /// <returns>
        /// Euclidean distance between two vectors.
        /// </returns>
        public static double EuclideanDistance(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("The vectors must be of the same length.");
            }

            double sum = 0;
            for (int i = 0; i < b.Length; i++)
            {
                sum += Math.Pow(a[i] - b[i], 2);
            }

            return Math.Sqrt(sum);
        }

        /// <summary> Projects the vector a onto b
        /// </summary>
        /// <returns>
        /// The length of a projected onto b
        /// </returns>
        public static double Projection(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("The vectors must be of the same length.");
            }

            // Get Unit vector for b
            double normB = EuclideanDistance(b, new double[] { 0, 0, 0 });
            double sum = 0;
            for (int i = 0; i < b.Length; i++)
            {
                sum += a[i] * (b[i] / normB);
            }

            return sum;
        }

        /// <summary> Gets the angle of the vector between the vector and its projection to the x-z plane.
        /// </summary>
        /// <returns>
        /// The angle between the vectors and the x-z plane.
        /// </returns>
        public static double AngleOfVector(double[] a)
        {
            double[] orgin = new double[] { 0, 0, 0 };
            double[] projectionA = new[] { a[0], 0, a[2] };
            double normA = EuclideanDistance(a, orgin);
            double normProjectionA = EuclideanDistance(projectionA, orgin);
            double dotProduct = 0;
            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * projectionA[i];
            }

            double result = Math.Acos(dotProduct / (normA * normProjectionA));
            if (double.IsNaN(result))
            {
                return 0;
            }
            else
            {
                return result;
            }
        }

        /// <summary>
        /// Subtracts two vectors as a - b
        /// </summary>
        /// <returns>
        /// A vector that is the result of the subtraction.
        /// </returns>
        public static double[] VectorDifference(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("The vectors must be of the same length.");
            }

            List<double> newVector = new List<double>();

            for (int i = 0; i < a.Length; i++)
            {
                newVector.Add(a[i] - b[i]);
            }

            return newVector.ToArray();
        }

        /// <summary>
        /// Calculates the midpoint between two vectors.
        /// </summary>
        /// <returns>
        /// The midpoint of the two vectors.
        /// </returns>
        public static double[] VectorMidpoint(double[] vectorA, double[] vectorB)
        {
            if (vectorA == null || vectorB == null)
            {
                throw new ArgumentNullException("vectorA");
            }

            if (vectorA.Length != vectorB.Length)
            {
                throw new ArgumentException("The vectors must be of the same length.");
            }

            double a = (vectorA[0] + vectorB[0]) / 2;
            double b = (vectorA[1] + vectorB[1]) / 2;
            double c = (vectorA[2] + vectorB[2]) / 2;

            return new[] { a, b, c };
        }

        /// <summary>
        /// Angle between two vectors
        /// </summary>
        /// <returns>
        /// The angle between the two vectors.
        /// </returns>
        public static double AngleBetween(double[] a, double[] b)
        {
            if (a.Length != b.Length)
            {
                throw new ArgumentException("The vectors must be of the same length.");
            }

            // Get vector norms 
            double normA = EuclideanDistance(a, new[] { 0.0, 0.0, 0.0 });
            double normB = EuclideanDistance(b, new[] { 0.0, 0.0, 0.0 });

            // Get the dot product
            double dotProduct = 0.0;
            for (int i = 0; i < a.Length; i++)
            {
                dotProduct += a[i] * b[i];
            }

            // Get angle with inverse cosine.
            return Math.Acos(dotProduct / (normA * normB));
        }

        /// <summary>
        /// Takes a list of data frames and returns a single data frame for each each non array 
        /// property is the average of the property across all data frames in the list.
        /// </summary>
        /// <param name="unaveragedFrames">
        /// List of frames to use for the average.
        /// </param>
        /// <returns>
        /// A single averaged frame.
        /// </returns>
        public static Frame AverageFrames(this IEnumerable<Frame> unaveragedFrames)
        {
            // Copy to a list to prevent multiple enumerations.
            List<Frame> frames = unaveragedFrames.ToList();
            Frame newFrame = frames.First();
            double tempDouble = 0.0;

            // Average every property for all five fingers
            for (int i = 0; i < 5; i++)
            {
                IEnumerable<PropertyInfo> nonArrayProps = newFrame.Fingers[i].GetType().GetProperties().Where(p => !p.PropertyType.IsArray);
                foreach (PropertyInfo prop in nonArrayProps)
                {
                    PropertyInfo propToSet = newFrame.Fingers[i].GetType().GetProperties().Single(p => p.Name.Equals(prop.Name));
                    foreach (Frame frame in frames)
                    {
                        object getVal = frame.Fingers[i].GetType().GetProperty(propToSet.Name).GetValue(frame.Fingers[i], null);
                        tempDouble += (double)getVal; // Cast to double
                    }

                    double newVal = tempDouble / frames.Count();
                    propToSet.SetValue(newFrame.Fingers[i], newVal);
                }
            }

            // Average every property for the palm
            IEnumerable<PropertyInfo> nonArrayPalmProps = newFrame.Palm.GetType().GetProperties().Where(p => !p.PropertyType.IsArray);
            tempDouble = 0.0;
            foreach (PropertyInfo prop in nonArrayPalmProps)
            {
                PropertyInfo propToSet = newFrame.Palm.GetType().GetProperties().Single(p => p.Name.Equals(prop.Name));
                foreach (Frame frame in frames)
                {
                    object getVal = frame.Palm.GetType().GetProperty(propToSet.Name).GetValue(frame.Palm, null);
                    tempDouble += (double)getVal; // Cast to double
                }

                double newVal = tempDouble / frames.Count();
                propToSet.SetValue(newFrame.Palm, newVal);
            }

            return newFrame;
        }
    }
}
