using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Train_DUT
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

    //S4
    public class evalCPU
    {
        string rootPath = @"C:\Users\pok\Semionline\S4\CPU_one_core\";
        string dataPath = "";
        string powerPath = "";
        private List<double> accFreq = new List<double>();
        
        private List<double> accTimeData0 = new List<double>();
        private List<double> accTimeData1 = new List<double>();
        private List<double> accTimeData2 = new List<double>();

        private List<double> accEntryData0 = new List<double>();
        private List<double> accEntryData1 = new List<double>();
        private List<double> accEntryData2 = new List<double>();

        private List<double> accUtil = new List<double>();

        private List<double> freqList = new List<double>();
        private List<double> idleList = new List<double>();
        private List<double> entryList = new List<double>();
        private List<double> utilList = new List<double>();
        private List<double> powerList = new List<double>();

        Dictionary<int, double> compare = new Dictionary<int, double>();

        private ArrayList trainSet = new ArrayList();
        private ArrayList testSet = new ArrayList();

        private ArrayList resultNotFound = new ArrayList();

        public evalCPU()
        {
            dataPath = rootPath + "data";
            powerPath = rootPath + "power";
        }

        public void execute()
        {

           
            string[] powerFiles = Directory.GetFiles(powerPath);

            trainSet.Add("test  freq  util  idleTime0  idleTime1  idleTime2  idleEntry0  idleEntry1  idleEntry2  power");
            //testSet.Add("freq util idleTime0 idleTime1 idleTime2 idleEntry0 idleEntry1 idleEntry2 power");
            
            for (int t = 1; t <= 1; t++)
            {

                        //POWER

                        double[] powers = Tool.powerParseArr(powerFiles[1], 0, powerFiles.Length, 5000);

                        ArrayList usePower = new ArrayList();
                        ArrayList unUsePower = new ArrayList();

                        List<double> pow = new List<double>();

                        for (int c = 0; c < powers.Length; c++)
                        {
                            if (powers[c] < 1250)
                                usePower.Add(powers[c]);
                            else
                                unUsePower.Add(powers[c]);
                        }


                        double sum = 0;
                        sum += (double)usePower[0];
                        for (int e = 1; e < usePower.Count; e++)
                        {
                            sum += (double)usePower[e];

                            if (e % 60 == 0)
                            {
                                pow.Add(sum / 60);
                                sum = 0;
                            }
                        }

                        //DATA
                      
                        string[] dataFiles = Directory.GetFiles(dataPath);

                        for (int d = 0; d < dataFiles.Length; d++)
                        {

                            string[] data = File.ReadAllLines(dataFiles[d]);

                            ArrayList filterData = new ArrayList();

                            for (int i = 0; i < data.Length; i++)
                            {
                                string _data = data[i];
                                if (!_data.Contains("bright=20.0")) continue;

                                filterData.Add(_data);
                            }

                            for (int x = 20; x < filterData.Count; x++)
                            {
                                string dat = ((string)filterData[x]).Trim();

                                if (dat.Contains("util=100")) continue;

                                string[] parameters = dat.Split(' ');

                                for (int p = 0; p < parameters.Length; p++)
                                {
                                    if (parameters[p].Equals("")) continue;

                                    string[] param = parameters[p].Split('=');

                                    switch (param[0])
                                    {
                                        case "util":
                                            double utilData = Double.Parse(param[1]);
                                            accUtil.Add(utilData);
                                            break;

                                        case "freq":
                                            double freqData = Double.Parse(param[1]);
                                            accFreq.Add(freqData);
                                            break;

                                        case "idle_time_s0":
                                            double idleTimeData0 = Double.Parse(param[1]);
                                            accTimeData0.Add(idleTimeData0);
                                            break;

                                        case "idle_time_s1":
                                            double idleTimeData1 = Double.Parse(param[1]);
                                            accTimeData1.Add(idleTimeData1);
                                            break;

                                        case "idle_time_s2":
                                            double idleTimeData2 = Double.Parse(param[1]);
                                            accTimeData2.Add(idleTimeData2);
                                            break;

                                        case "idle_entry_s0":
                                            double idleEntryData0 = Double.Parse(param[1]);
                                            accEntryData0.Add(idleEntryData0);
                                            break;

                                        case "idle_entry_s1":
                                            double idleEntryData1 = Double.Parse(param[1]);
                                            accEntryData1.Add(idleEntryData1);
                                            break;

                                        case "idle_entry_s2":
                                            double idleEntryData2 = Double.Parse(param[1]);
                                            accEntryData2.Add(idleEntryData2);
                                            break;
                                    }

                                }

                            }

                            double freq = Math.Round(accFreq.Max());
                            double util = Math.Round(accUtil.Mean(),2);
             
                            double idleTime0 = Math.Round(accTimeData0.Average(),2);
                            double idleTime1 = Math.Round(accTimeData1.Average(),2);
                            double idleTime2 = Math.Round(accTimeData2.Average(),2);

                            double idleEntry0 = Math.Round(accEntryData0.Average());
                            double idleEntry1 = Math.Round(accEntryData1.Average());
                            double idleEntry2 = Math.Round(accEntryData2.Average());

                            double power = Math.Round(pow[d],2);

                            trainSet.Add(t + "  " + freq + "  " + util + "  " + idleTime0 + "  " + idleTime1 + "  " + idleTime2 + "  " + idleEntry0 + "  " + idleEntry1 + "  " + idleEntry2 + "  " + power);

                          
                            accFreq.Clear();
                            accUtil.Clear();

                            accTimeData0.Clear();
                            accTimeData1.Clear();
                            accTimeData2.Clear();

                            accEntryData0.Clear();
                            accEntryData1.Clear();
                            accEntryData2.Clear();
                            
                            
                      }

                        string[] trainData = (string[])trainSet.ToArray(typeof(string));


                        string saveFile = rootPath + @"\output\train.txt";

                        File.WriteAllLines(saveFile, trainData);

                             
            }
        }
    }
}
    
    

