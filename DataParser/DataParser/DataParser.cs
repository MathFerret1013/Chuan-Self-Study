namespace DataParser
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;

    /// <summary>
    /// This class is responsible for parsing data-frame from a text file.
    /// Data frames are parsed line by line
    /// </summary>
    public class DataParser
    {
        /// <summary>
        /// Regular expression to match data frames.
        /// </summary>
        private const string DataframeRegex = @".+?\s\.\s";

        /// <summary>
        /// SteamReader object that actually reads the file.
        /// </summary>
        private readonly StreamReader fileStreamReader;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataParser"/> class. 
        /// Initializes a new DataParser class with the given file path as the file to read from.
        /// </summary>
        /// <param name="filePath"> Path to the file that contains the raw data
        /// </param>
        public DataParser(string filePath)
        {
            this.FilePath = filePath;
            this.fileStreamReader = new StreamReader(this.FilePath);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="DataParser"/> class. 
        /// Close the file stream before finalize is called.
        /// </summary>
        ~DataParser()
        {
            this.fileStreamReader.Close();
        }

        /// <summary> Path to the text file to be read.
        /// </summary>
        public string FilePath { get; private set; }

        /// <summary> The current line being operated on by the DataParser
        /// </summary>
        public string CurrentLine { get; private set; }

        /// <summary> Takes in string containing data. Strips all parenthesis and semi-colons.
        /// </summary>
        /// <param name="data"> Raw data from LEAP motion device.
        /// </param>
        /// <returns> Float array containing numerical values.
        /// </returns>
        public static double[] ParseDataString(string data)
        {
            string tempStr = string.Empty;
            List<char> removeList = new List<char> { '(', ')', ';' };
            int i = 0;

            // Remove all characters in the remove list
            while (i < data.Length)
            {
                if (!removeList.Contains(data[i]))
                {
                    tempStr += data[i];
                }

                i++;
            }

            // split on commas
            double[] floats = tempStr.Split(',').Select(double.Parse).ToArray();
            return floats;
        }

        /// <summary> Gets the next line in the file. If there are no more lines then null is returned.
        /// </summary>
        /// <returns> Boolean indicating if a line was successfully read.
        /// </returns>
        public bool MoveToNextLine()
        {
            this.CurrentLine = this.fileStreamReader.ReadLine();
            return !string.IsNullOrEmpty(this.CurrentLine);
        }

        /// <summary>
        /// Gets all data frames for the current line.
        /// A data frame is denoted by a ". "
        /// One data frame contains data for all five fingers
        /// </summary>
        /// <param name="averagingSize">
        /// Number of frames to average together
        /// </param>
        /// <param name="averageFrames">
        /// Determines if the frames should be averaged together
        /// </param>
        /// <returns>
        /// IEnumerable of data frames.
        /// </returns>
        public IEnumerable<Frame> GetDataFrames(int averagingSize = 5, bool averageFrames = false)
        {
            Regex regex = new Regex(DataframeRegex);
            MatchCollection matches = regex.Matches(this.CurrentLine);
            char sign = this.CurrentLine.Last();
            List<Frame> dataFrames = new List<Frame>();

            List<string> dataframeStrings = new List<string>();

            foreach (Match match in matches)
            {
                dataframeStrings.Add(match.Value.TrimEnd(new[] { '.', ' ' }));
            }

            foreach (string dataframeString in dataframeStrings)
            {
                dataFrames.Add(new Frame(dataframeString, sign));
            }

            this.CalculateDistance(dataFrames);
            this.CalculateFingerOrder(dataFrames);
            this.CalculateSpread(dataFrames);
            this.CalculateTriSpread(dataFrames);

            if (averageFrames)
            {
                char currentSign = dataFrames.First().Sign;
                List<Frame> tempList = new List<Frame>();
                List<Frame> averagedFrames = new List<Frame>();

                // Get iterator for frames of the current sign
                IEnumerator<Frame> frameIter = dataFrames.Where(f => f.Sign.Equals(currentSign)).GetEnumerator();
                int i = 0;
                while (frameIter.MoveNext())
                {
                    tempList.Add(frameIter.Current);
                    i++;

                    if (i == averagingSize)
                    {
                        // If we have five frames
                        averagedFrames.Add(tempList.AverageFrames());
                        tempList.Clear();
                        i = 0;
                    }
                }

                // If there are any un averaged frames average them now
                if (tempList.Count > 0)
                {
                    averagedFrames.Add(tempList.AverageFrames());
                }

                return averagedFrames;
            }

            return dataFrames;
        }

        /// <summary>
        /// Calculates the average distance traveled between frames.
        /// </summary>
        /// <param name="dataFrames">
        /// The data Frames.
        /// </param>
        public void CalculateDistance(IList<Frame> dataFrames)
        {
            List<double> distances = new List<double>();

            // Calculate distance of finger tip positions between frames
            for (int i = 0; i < dataFrames.Count - 1; i++)
            {
                // for each frame
                for (int j = 0; j < 5; j++)
                {
                    // for each finger
                    distances.Add(VectorCalculator.EuclideanDistance(dataFrames[i].Fingers[j].TipPosition, dataFrames[i + 1].Fingers[j].TipPosition));
                }
            }

            double averageDistance = distances.Average();
            for (int i = 0; i < dataFrames.Count; i++)
            {
                dataFrames[i].AverageDistance = double.IsNaN(averageDistance) ? 0.0 : averageDistance;
            }
        }

        /// <summary>
        /// Calculates the average finger order of each finger across all data frames.
        /// </summary>
        /// <param name="dataFrames">Data frames to perform calculations on.</param>
        public void CalculateFingerOrder(IList<Frame> dataFrames)
        {
            double[] orders = new double[5];
            double orderSum = 0;

            for (int j = 0; j < 5; j++)
            {
                // for each finger
                for (int i = 0; i < dataFrames.Count; i++)
                {
                    // for each frame
                    orderSum += dataFrames[i].Fingers[j].Order;
                }

                orders[j] = orderSum / dataFrames.Count;
                orderSum = 0;
            }

            for (int i = 0; i < dataFrames.Count; i++)
            {
                // for each frame
                for (int j = 0; j < 5; j++)
                {
                    // for each finger
                    dataFrames[i].Fingers[j].Order = orders[j];
                }
            }
        }

        /// <summary>
        /// Calculates the average "spread" (space between fingers)
        /// </summary>
        /// <param name="dataFrames">Data frames to perform calculations on.</param>
        public void CalculateSpread(IList<Frame> dataFrames)
        {
            List<double> spreads = new List<double>();
            for (int i = 0; i < dataFrames.Count; i++)
            {
                spreads.Add(dataFrames[i].GetSpread());
            }

            double averageSpread = spreads.Average();

            for (int i = 0; i < dataFrames.Count; i++)
            {
                dataFrames[i].AverageSpread = double.IsNaN(averageSpread) ? 0.0 : averageSpread;
            }
        }

        /// <summary>
        /// Calculates the average "tri-spread" (triangle between finger tips and metacarpal midpoints)
        /// </summary>
        /// <param name="dataFrames"> Data frames to perform calculations on.
        /// </param>
        public void CalculateTriSpread(IList<Frame> dataFrames)
        {
            List<double> triSpreads = new List<double>();
            for (int i = 0; i < dataFrames.Count; i++)
            {
                triSpreads.Add(dataFrames[i].GetTriangularSpread());
            }

            double averageSpread = triSpreads.Average();

            for (int i = 0; i < dataFrames.Count; i++)
            {
                dataFrames[i].AverageTriangularSpread = double.IsNaN(averageSpread) ? 0.0 : averageSpread;
            }
        }
    }
}