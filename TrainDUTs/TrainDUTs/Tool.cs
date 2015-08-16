using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Parse;
using System.IO;
using System.Windows.Forms;
using System.Threading;
using System.Collections;

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
        public static TextBox tbStatus;

        public static void init(TextBox tb)
        {
            tbStatus = tb;             
        }

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
                
        public static void showStatus(string statusMessage)
        {
            tbStatus.AppendText(statusMessage + "\n");
            tbStatus.Update();
        }

        //1. Run sample.o
        //2. Wait for 5 seconds.
        //3. Run monsoon 
        //4. Wait for 5 seconds
        //5. Trigger display brightness from on to off, wait for 1 second, set from off to on.
        //6. Wait for 4 seconds
        //7. Start activity that we want to test.

        public static void sampleData()
        {
            int fIndex = Config.fileIndex;
            int duration = Config.duration;

            //1
            //set DUT brightness to low.
            //showStatus("Set brightness 0");
            //Config.callProcess("echo 0 > " + Config.brightPath);

            //2
            Thread a = new Thread(() => Config.callProcess("/data/local/tmp/sample " + fIndex + " " + duration + " 1 0 &"));
            a.Start();
            //showStatus("Call sample " + fIndex);
            //Config.callProcess3("adb shell /data/local/tmp/sample " + fIndex + " " + duration + " 1 0 &");
          

          
            //3
            //showStatus("Set brightness 255");
            //Config.callProcess("echo 255 > " + Config.brightPath);
            
            //4
            //showStatus("Call monsoon");
            Thread b = new Thread(() => monsoonTask());
            //b.Start();
            //thread.Start();
                     
            //5
            showStatus("5. Start activity");
                        
        }

        public static void monsoonTask()
        {
            Thread.Sleep(10000);
            Config.callPowerMeter(Config.rootPath + "power" + Config.fileIndex + ".pt4", Config.duration);

        }

        public static void ParsePower()
        {

            double[] powers = Tool.powerParseArr(Config.rootPath + @"\3g_3.pt4", 0);

            string[] powerStrs = Array.ConvertAll(powers, element => element.ToString());
            File.WriteAllLines(Config.rootPath + @"\3g_3.txt", powerStrs);

        }
/*
        public static void ParseData_nexus()
        {

            int fileIndex = Config.fileIndex;
            int fileIndexEnd = Config.duration;

            string savePath = Config.rootPath;

            string inputFileName = savePath + "sample" + fileIndex + ".txt";
            string[] dataFiles = Directory.GetFiles(inputFileName);
            string[] dataNames = new string[dataFiles.Length];
            for (int i = 0; i < dataFiles.Length; i++)
                dataNames[i] = Path.GetFileNameWithoutExtension(dataFiles[i]);

            string[] powerFiles = Directory.GetFiles(savePath + "power");
            string[] powerNames = new string[powerFiles.Length];
            for (int i = 0; i < powerFiles.Length; i++)
                powerNames[i] = Path.GetFileNameWithoutExtension(powerFiles[i]);
            
            //string header = "util freq idle_time idle_usage bright tx rx up ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power";
            string header = "util0 freq0 it ie bright tx rx status capacity volt temp ftime fps gtl2d_core gtl3d_core gtlcom_core gtlta_core gtt2d_core gtt3d_core gttcom_core gttta_core spm ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf vps";

            List<List<string>> lists = new List<List<string>>();

            ArrayList saveData = new ArrayList();

            for (int i = 0; i < dataNames.Length; i++)
            {

                string inputFile = dataFiles[i];
                
                if (!File.Exists(inputFile))
                {
                    MessageBox.Show("File not found exception: " + inputFile);
                    System.Environment.Exit(-1);
                }

                string[] datas = File.ReadAllLines(inputFile);

                //int resultIndex = Array.BinarySearch<string>(powerNames, dataNames[i]);
                double[] powers = Tool.powerParseArr(savePath + @"power\"+dataNames[i]+@".pt4", 0);

                lists = Config.processData(datas);

                int row = lists.Count - 1;
                int col = 0; // lists[0].Count;
                string values = "";
                saveData.Add(header);

                for (int r = Config.offset; r < row; r++)
                {
                    List<string> curData = lists[r];
                    col = curData.Count;

                    for (int c = 1; c < col; c++)
                    {
                        values += curData[c] + " ";
                    }

                    values += powers[r-Config.offset];
                    saveData.Add(values);
                    values = "";
                }

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                string saveName = Config.rootPath + "parse\\" + dataNames[i] + ".txt";
                Console.WriteLine("File save = " + saveName);
                File.WriteAllLines(saveName, toSave);
                saveData.Clear();

                //MessageBox.Show("File raw_data_" + i + ".txt is saved at " + Config.rootPath);
            }
        }
*/
        public static void ParseData()
        {

            int fileIndex = Config.fileIndex;
            int fileIndexEnd = Config.duration;

            //Config.fileIndex = int.Parse(fileIndex_tb.Text);
            //Config.duration = int.Parse(sampleTime_tb.Text);

            //Config.callProcess2("pull data/local/tmp/stat/sample"+fileIndex+@".txt " + Config.rootPath+"sample"+fileIndex+@".txt");

            string savePath = Config.rootPath;
            
            string header = "";

            if(Config.DUT == 1) //S4
            {   
                 header = "util0 util1 util2 util3 freq0 freq1 freq2 freq3 " +
                "it0s0 it0s1 it0s2 it1s0 it1s1 it1s2 it2s0 it2s1 it2s2 it3s0 it3s1 it3s2 " +
                "iu0s0 iu0s1 iu0s2 iu1s0 iu1s1 iu1s2 iu2s0 iu2s1 iu2s2 iu3s0 iu3s1 iu3s2 " +
                "mem " +
                "bright " +
                "tx rx status " +
                "volt current capacity temp " +
                "ftime fps gtl2d_core gtl3d_core gtac_core gtlta_core gtt2d_core gtt3d_core gttc_core gttta_core spm isp ta_load usse_cc_pp usse_cc_pv usse_load_p usse_load_v vpf vps power";
            }
            else //Nexus s
            {
                header = "util0 freq0 it0s0 iu0s0 mem bright tx rx status volt capacity temp ftime fps gtl2d_core gtl3d_core gtlcom_core gtlta_core gtt2d_core gtt3d_core gttcom_core gttta_core spm ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf vps power";
            }

            List<List<string>> lists = new List<List<string>>();

            ArrayList saveData = new ArrayList();

            for (int i = fileIndex; i <= fileIndexEnd; i++)
            {

                string inputFileName = savePath + "sample" + i + ".txt";
                if (!File.Exists(inputFileName))
                {
                    MessageBox.Show("File not found exception: " + inputFileName);
                    System.Environment.Exit(-1);
                }
                string[] datas = File.ReadAllLines(inputFileName);
                double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                
                //remove previous data more than 100
                int ck = 0;
                for (int i1 = 0; i1 < powers.Length; i1++)
                {
                    double p = powers[i1];

                    if (p < 100)
                    {
                        ck = i1;
                    }
                }

                ++ck;
                double[] powerNews = new double[powers.Length-ck];
                for (int i2 = ck; i2 < powers.Length; ++i2 )
                {
                    powerNews[i2-ck] = powers[i2];
                }
                    
                lists = Config.processData(datas);

                int row = lists.Count - 1;
                int col = 0; // lists[0].Count;
                string values = "";
                saveData.Add(header);

                int ck2 = 0;
                for (int r = 1; r < row; r++)
                {

                    List<string> curData = lists[r];
                    col = curData.Count;

                    int brightIndex = -1;
                    if (Config.DUT == 1)
                        brightIndex = 15;
                    else
                        brightIndex = 6;

                    string test = curData[brightIndex];
                    if (test.Equals("0") || test.Equals("10"))
                    {
                        ck2 = r;
                    }
                    
                }
                ++ck2;

                int pi = 0;

                int lenTest = 0;
                if (powerNews.Length < lists.Count)
                    lenTest = powerNews.Length;
                else
                    lenTest = lists.Count;

                

                for (int r = ck2; r < lenTest; r++)
                {

                    if (r == lenTest - 1)
                    {
                        Console.WriteLine("");
                    }

                    List<string> curData = lists[r];

                    col = curData.Count;

                    for (int c = 1; c < col; c++)
                    {

                        values += curData[c] + " ";
                    }

                    if (pi == lenTest) break;

                    values += powerNews[pi];
                    ++pi;
                    saveData.Add(values);

                    values = "";
                }

              

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                string saveName = Config.rootPath + "raw_data_" + i + ".txt";
                Console.WriteLine("File save = " + saveName);
                File.WriteAllLines(saveName, toSave);
                saveData.Clear();

                //MessageBox.Show("File raw_data_"+fileIndex+".txt is saved at "+Config.rootPath);
            }

            //MessageBox.Show("File raw_data_" + fileIndex + "-" + fileIndexEnd + ".txt is saved at " + Config.rootPath);
            Application.Exit();
        }
        
       
    }
}
