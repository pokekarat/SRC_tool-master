using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using System.IO;

namespace TrainDUTs
{
    public static class MyListExtensions
    {
        public static double Median(this IEnumerable<double> list)
        {
            List<double> orderedList = list
                .OrderBy(numbers => numbers)
                .ToList();

            int listSize = orderedList.Count;
            double result;

            if (listSize % 2 == 0) // even
            {
                int midIndex = listSize / 2;
                result = ((orderedList.ElementAt(midIndex - 1) +
                           orderedList.ElementAt(midIndex)) / 2);
            }
            else // odd
            {
                double element = (double)listSize / 2;
                element = Math.Round(element, MidpointRounding.AwayFromZero);

                result = orderedList.ElementAt((int)(element - 1));
            }

            return result;
        }

        public static double Mean(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.Mean(0, values.Count);
        }

        public static double Mean(this List<double> values, int start, int end)
        {
            double s = 0;

            for (int i = start; i < end; i++)
            {
                s += values[i];
            }

            return s / (end - start);
        }

        public static double Variance(this List<double> values)
        {
            return values.Variance(values.Mean(), 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean)
        {
            return values.Variance(mean, 0, values.Count);
        }

        public static double Variance(this List<double> values, double mean, int start, int end)
        {
            double variance = 0;

            for (int i = start; i < end; i++)
            {
                variance += Math.Pow((values[i] - mean), 2);
            }

            int n = end - start;
            if (start > 0) n -= 1;

            return variance / (n);
        }

        public static double StandardDeviation(this List<double> values)
        {
            return values.Count == 0 ? 0 : values.StandardDeviation(0, values.Count);
        }

        public static double StandardDeviation(this List<double> values, int start, int end)
        {
            double mean = values.Mean(start, end);
            double variance = values.Variance(mean, start, end);

            return Math.Sqrt(variance);
        }

        public static IEnumerable<double> Modes(this IEnumerable<double> list)
        {
            var modesList = list
                .GroupBy(values => values)
                .Select(valueCluster =>
                        new
                        {
                            Value = valueCluster.Key,
                            Occurrence = valueCluster.Count(),
                        })
                .ToList();

            int maxOccurrence = modesList
                .Max(g => g.Occurrence);

            return modesList
                .Where(x => x.Occurrence == maxOccurrence && maxOccurrence > 1) // Thanks Rui!
                .Select(x => x.Value);
        }
    }

    class Tool
    {
        static double powerSum = 0;
        static double powerAvg = 0;
        static double powerCnt = 0;

        public static double powerParse(string file, int beginIndex, int avgDuration)
        {
            string input = file;

            FileStream pt4Stream = File.Open(
                                                 input,
                                                  FileMode.Open,
                                                  FileAccess.Read,
                                                  FileShare.ReadWrite
                                              );

            //Console.WriteLine("File source " + args[1]);

            BinaryReader pt4Reader = new BinaryReader(pt4Stream);

            // reader the file header
            PT4.Pt4Header header = new PT4.Pt4Header();

            PT4.ReadHeader(pt4Reader, ref header);

            // read the Status Packet
            PT4.StatusPacket statusPacket = new PT4.StatusPacket();
            PT4.ReadStatusPacket(pt4Reader, ref statusPacket);

            // determine the number of samples in the file
            long sampleCount = PT4.SampleCount(pt4Reader, header.captureDataMask);

            // pre-position input file to the beginning of the sample // data (saves a lot of repositioning in the GetSample // routine)
            pt4Reader.BaseStream.Position = PT4.sampleOffset;
            // process the samples sequentially, beginning to end
            PT4.Sample sample = new PT4.Sample();

            int startTime = beginIndex * avgDuration;

            for (long sampleIndex = startTime; sampleIndex < sampleCount; sampleIndex++)
            {
                PT4.GetSample(sampleIndex, header.captureDataMask, statusPacket, pt4Reader, ref sample);
                powerSum += (sample.mainCurrent * sample.mainVoltage);
                powerCnt++;
            }

            powerAvg = powerSum / powerCnt;
            pt4Reader.Close();

            return powerAvg;
        }

        public static double[] powerParseArr(int powerIndex, string folder, int beginIndex, int avgDuration)
        {
            string input = folder + @"\power" + powerIndex + ".pt4";

            FileStream pt4Stream = File.Open(
                                                 input,
                                                  FileMode.Open,
                                                  FileAccess.Read,
                                                  FileShare.ReadWrite
                                              );

            //Console.WriteLine("File source " + args[1]);

            BinaryReader pt4Reader = new BinaryReader(pt4Stream);

            // reader the file header
            PT4.Pt4Header header = new PT4.Pt4Header();

            PT4.ReadHeader(pt4Reader, ref header);

            // read the Status Packet
            PT4.StatusPacket statusPacket = new PT4.StatusPacket();
            PT4.ReadStatusPacket(pt4Reader, ref statusPacket);

            // determine the number of samples in the file
            long sampleCount = PT4.SampleCount(pt4Reader, header.captureDataMask);

            // pre-position input file to the beginning of the sample // data (saves a lot of repositioning in the GetSample // routine)
            pt4Reader.BaseStream.Position = PT4.sampleOffset;
            // process the samples sequentially, beginning to end
            PT4.Sample sample = new PT4.Sample();

            powerSum = 0;
            powerCnt = 0;
            powerAvg = 0;
            double[] results = new double[sampleCount / avgDuration];

            for (long sampleIndex = beginIndex; sampleIndex < sampleCount; sampleIndex++)
            {
                PT4.GetSample(sampleIndex, header.captureDataMask, statusPacket, pt4Reader, ref sample);
                powerSum += sample.mainCurrent * sample.mainVoltage;
                ++powerCnt;

                if (powerCnt == avgDuration)
                {
                    powerAvg = powerSum / powerCnt;
                    results[((sampleIndex + 1) / avgDuration) - 1] = powerAvg;
                    powerSum = 0;
                    powerCnt = 0;
                }
            }

            return results;
        }

        public static double[] powerParseArr(string pt4file, int start, int stop, int samplingRate)
        {
            string input = pt4file;

            FileStream pt4Stream = File.Open(
                                                 input,
                                                  FileMode.Open,
                                                  FileAccess.Read,
                                                  FileShare.ReadWrite
                                              );

            //Console.WriteLine("File source " + args[1]);

            BinaryReader pt4Reader = new BinaryReader(pt4Stream);

            // reader the file header
            PT4.Pt4Header header = new PT4.Pt4Header();

            PT4.ReadHeader(pt4Reader, ref header);

            // read the Status Packet
            PT4.StatusPacket statusPacket = new PT4.StatusPacket();
            PT4.ReadStatusPacket(pt4Reader, ref statusPacket);

            // determine the number of samples in the file
            long sampleCount = PT4.SampleCount(pt4Reader, header.captureDataMask);

            // pre-position input file to the beginning of the sample // data (saves a lot of repositioning in the GetSample // routine)
            pt4Reader.BaseStream.Position = PT4.sampleOffset;
            // process the samples sequentially, beginning to end
            PT4.Sample sample = new PT4.Sample();



            //double[] results = new double[sampleCount / samplingRate];

            List<double> ret = new List<double>();
            //if (size == 0) size = (int)sampleCount;

            int startTime = start * samplingRate;
            int stopTime = stop * samplingRate;

            for (long sampleIndex = startTime; sampleIndex < sampleCount; sampleIndex++)
            {
                PT4.GetSample(sampleIndex, header.captureDataMask, statusPacket, pt4Reader, ref sample);
                powerSum += sample.mainCurrent * sample.mainVoltage;
                ++powerCnt;

                if (powerCnt == samplingRate)
                {
                    powerAvg = powerSum / powerCnt;
                    //results[((sampleIndex + 1) / samplingRate) - 1] = powerAvg;
                    ret.Add(powerAvg);
                    powerSum = 0;
                    powerCnt = 0;
                }
            }

            return ret.ToArray();
        }

        public static double[] powerParseArr(string pt4file, int start)
        {
            string input = pt4file;

            FileStream pt4Stream = File.Open(
                                                 input,
                                                  FileMode.Open,
                                                  FileAccess.Read,
                                                  FileShare.ReadWrite
                                              );

            //Console.WriteLine("File source " + args[1]);

            BinaryReader pt4Reader = new BinaryReader(pt4Stream);

            // reader the file header
            PT4.Pt4Header header = new PT4.Pt4Header();

            PT4.ReadHeader(pt4Reader, ref header);

            // read the Status Packet
            PT4.StatusPacket statusPacket = new PT4.StatusPacket();
            PT4.ReadStatusPacket(pt4Reader, ref statusPacket);

            // determine the number of samples in the file
            long sampleCount = PT4.SampleCount(pt4Reader, header.captureDataMask);

            // pre-position input file to the beginning of the sample // data (saves a lot of repositioning in the GetSample // routine)
            pt4Reader.BaseStream.Position = PT4.sampleOffset;
            // process the samples sequentially, beginning to end
            PT4.Sample sample = new PT4.Sample();

            List<double> ret = new List<double>();

            int begin = start * 5000;
            for (long sampleIndex = begin; sampleIndex < sampleCount; sampleIndex++)
            {
                PT4.GetSample(sampleIndex, header.captureDataMask, statusPacket, pt4Reader, ref sample);
                powerSum += (sample.mainCurrent * sample.mainVoltage);
                ++powerCnt;

                if (powerCnt == 5000)
                {
                    powerAvg = powerSum / powerCnt;
                    //results[((sampleIndex + 1) / samplingRate) - 1] = powerAvg;
                    ret.Add(powerAvg);
                    powerSum = 0;
                    powerCnt = 0;
                }
            }

            return ret.ToArray();
        }

    }
}
