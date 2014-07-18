namespace DataParser
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using System.Web.Script.Serialization;

    /// <summary>
    /// Main entry point for data parser
    /// </summary>
    public class Program
    {
        /// <summary>
        /// Main method
        /// </summary>
        /// <param name="args">
        /// The command line args.
        /// </param>
        public static void Main(string[] args)
        {
            // Input data to be parsed. The last file is always reserved for training data.
            List<string> inputPaths = new List<string>()
            {
                @"C:\Users\eric\Dropbox\SVN_FILES\School Projects\Chuan Self Study\Sample Data\data-gold1-Guardino.txt", 
                @"C:\Users\Eric\Dropbox\SVN_FILES\School Projects\Chuan Self Study\Sample Data\Jon-gold-2.txt", 
                @"C:\Users\Eric\Dropbox\SVN_FILES\School Projects\Chuan Self Study\Sample Data\Jon-gold-1.txt", 
                @"C:\Users\eric\Dropbox\SVN_FILES\School Projects\Chuan Self Study\Sample Data\data-gold2-Guardino.txt"
            };

            // There should only ever be two output files.
            List<string> outputPaths = new List<string>()
            {
                @"C:\Users\eric\Dropbox\SVN_FILES\School Projects\Chuan Self Study\Sample Data\TrainingData.json", 
                @"C:\Users\eric\Dropbox\SVN_FILES\School Projects\Chuan Self Study\Sample Data\TestData.json"
            };

            StringBuilder jsonStringBuilder = new StringBuilder();
            int j = 0;
            for (int i = 0; i < inputPaths.Count; i++)
            {
                using (StreamWriter file = new StreamWriter(outputPaths[j]))
                {
                    DataParser parser = new DataParser(inputPaths[i]);
                    while (parser.MoveToNextLine())
                    {
                        // While the line is not empty
                        JavaScriptSerializer serializer = new JavaScriptSerializer();
                        IEnumerable<Frame> frames = parser.GetDataFrames(); // Call GetDataFrames with parameters to us a moving average
                        foreach (Frame frame in frames)
                        {
                            // All attributes that will be exported to JSON.
                            // The attributes will be exported in the order they appear
                            // IT IS CRUCIAL THAT THE SIGN ATTRIBUTE BE LAST, the R script relies on this.
                            jsonStringBuilder.AppendLine(serializer.Serialize(new
                                                                                  {
                                                                                      Finger1ExtendedDistance =
                                                                                          frame.Fingers[0]
                                                                                              .DistanceFromPalmCenter, 
                                                                                      Finger1DipTipProjection =
                                                                                          frame.Fingers[0]
                                                                                              .DipTipProjection, 
                                
                                                                                      // Finger1PipDipProjection = frame.Fingers[0].PipDipProjection,
                                                                                      // Finger1McpPipProjection = frame.Fingers[0].McpPipProjection,
                                                                                      Finger1OrderX =
                                                                                          frame.Fingers[0].Order, 
                                                                                      Finger1Angle =
                                                                                          frame.Fingers[0].Angle, 
                                                                                      Finger2ExtendedDistance =
                                                                                          frame.Fingers[1]
                                                                                              .DistanceFromPalmCenter, 
                                                                                      Finger2DipTipProjection =
                                                                                          frame.Fingers[1]
                                                                                              .DipTipProjection, 
                                                                                      
                                                                                      // Finger2PipDipProjection = frame.Fingers[1].PipDipProjection,
                                                                                      // Finger2McpPipProjection = frame.Fingers[1].McpPipProjection,
                                                                                      Finger2OrderX =
                                                                                          frame.Fingers[1].Order, 
                                                                                      Finger2Angle =
                                                                                          frame.Fingers[1].Angle, 
                                                                                      Finger3ExtendedDistance =
                                                                                          frame.Fingers[2]
                                                                                              .DistanceFromPalmCenter, 
                                                                                      Finger3DipTipProjection =
                                                                                          frame.Fingers[2]
                                                                                              .DipTipProjection, 
                                                                                      
                                                                                      // Finger3PipDipProjection = frame.Fingers[2].PipDipProjection,
                                                                                      // Finger3McpPipProjection = frame.Fingers[2].McpPipProjection,
                                                                                      Finger3OrderX =
                                                                                          frame.Fingers[2].Order, 
                                                                                      Finger3Angle =
                                                                                          frame.Fingers[2].Angle, 
                                                                                      Finger4ExtendedDistance =
                                                                                          frame.Fingers[3]
                                                                                              .DistanceFromPalmCenter, 
                                                                                      Finger4DipTipProjection =
                                                                                          frame.Fingers[3]
                                                                                              .DipTipProjection, 
                                                                                      
                                                                                      // Finger4PipDipProjection = frame.Fingers[3].PipDipProjection,
                                                                                      // Finger4McpPipProjection = frame.Fingers[3].McpPipProjection,
                                                                                      Finger4OrderX =
                                                                                          frame.Fingers[3].Order, 
                                                                                      Finger4Angle =
                                                                                          frame.Fingers[3].Angle, 
                                                                                      Finger5ExtendedDistance =
                                                                                          frame.Fingers[4]
                                                                                              .DistanceFromPalmCenter, 
                                                                                      Finger5DipTipProjection =
                                                                                          frame.Fingers[4]
                                                                                              .DipTipProjection, 
                                                                                      
                                                                                      // Finger5PipDipProjection = frame.Fingers[4].PipDipProjection,
                                                                                      // Finger5McpPipProjection = frame.Fingers[4].McpPipProjection,
                                                                                      Finger5OrderX =
                                                                                          frame.Fingers[4].Order, 
                                                                                      Finger5Angle =
                                                                                          frame.Fingers[4].Angle, 
                                                                                      PalmGrabStrength =
                                                                                          frame.Palm.GrabStrength, 
                                                                                      PalmPinchStrength =
                                                                                          frame.Palm.PinchStrength,
                                                                                      frame.AverageDistance,
                                                                                      frame.AverageSpread, 
                                                                                      AverageTriSpread =
                                                                                          frame.AverageTriangularSpread, 

                                                                                      // THIS MUST BE THE LAST FEATURE
                                                                                      frame.Sign
                                                                                  }));
                        }
                    }

                    // If we have finished parsing the training data, the set the next output file to testing data
                    if (i >= inputPaths.Count - 2)
                    {
                        file.Write(jsonStringBuilder);
                        jsonStringBuilder.Clear();
                        j = 1;
                    }
                }
            }

            Console.WriteLine("DONE");
            Console.ReadLine();
        }
    }
}
