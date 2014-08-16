using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Parse;
using System.Collections;
namespace Train_DUT
{
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
