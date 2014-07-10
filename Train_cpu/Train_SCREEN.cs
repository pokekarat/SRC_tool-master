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

    public class Train_SCREEN
    {
        string samplePath = @"D:\SemiOnline\Experiment\Nexus\CPU_idle";
        private List<double> accFreq = new List<double>();
        private List<double> accIdleTime = new List<double>();
        private List<double> accEntryData = new List<double>();
        private List<double> accUtil = new List<double>();

        private List<double> freqList = new List<double>();
        private List<double> idleList = new List<double>();
        private List<double> entryList = new List<double>();
        private List<double> utilList = new List<double>();
        private List<double> powerList = new List<double>();

        Dictionary<int, double> compare = new Dictionary<int, double>();

        public Train_SCREEN()
        {
            
        }

        public void execute()
        {

            /*string[] allFiles = Directory.GetFiles(samplePath);
           
            ArrayList sampleData = new ArrayList();
            ArrayList samplePower = new ArrayList();

            for (int i = 0; i < allFiles.Length; i++)
            {
                if (allFiles[i].Contains(".txt"))
                {
                    sampleData.Add(allFiles[i]);
                }

                if (allFiles[i].Contains(".pt4"))
                {
                    samplePower.Add(allFiles[i]);
                }
            }*/

            string[] freqs = { "200000", "400000", "800000" };
            string[] utils = { "1", "25", "50", "75" };
            string[] idles = {"1","10","20","50","80","100","200","500","800","1000"};

           

            for(int d=0; d<idles.Length; d++)
            {
                for (int u = 0; u < utils.Length; u++)
                {
                    for (int f = 0; f < freqs.Length; f++)
                    {
                        
                        Console.WriteLine("\nFile = freq_" + freqs[f] + "_util_" + utils[u] + "_" + idles[d] + ".txt");

                        for (int t = 1; t <= 7; t++)
                        {
                            string dataFile = @"\test_" + t + "_freq_" + freqs[f] + "_util_" + utils[u]+"_"+idles[d]+".txt";
                            string powerFile = @"\test_" + t + "_freq_" + freqs[f] + "_util_" + utils[u] + "_idle_" + idles[d] + ".pt4";

                            double power = Tool.powerParse(samplePath + powerFile, 10);

                            string[] data = File.ReadAllLines(samplePath + dataFile);

                            for (int x = 0; x < data.Length; x++)
                            {
                                if (x < 10 || x == data.Length - 1)
                                {
                                    continue;
                                }

                                string[] param = data[x].Split(' ');

                                string[] util = param[2].Split('=');
                                double utilData = Double.Parse(util[1]);
                                accUtil.Add(utilData);
                               
                                string[] freq = param[5].Split('=');
                                double freqData = Double.Parse(freq[1]);
                                accFreq.Add(freqData);
                                   
                                string[] idleTime = param[3].Split('=');
                                double idleTimeData = Double.Parse(idleTime[1]);
                                accIdleTime.Add(idleTimeData);
                                  
                                string[] entry = param[4].Split('=');
                                double entryData = Double.Parse(entry[1]);
                                accEntryData.Add(entryData);
                                    
                            }

                            double util1 = accUtil.Median();
                            double idle1 =  accIdleTime.Median();
                            double freq1 = accFreq.Median();
                            double entry1 = accEntryData.Median();
                            double total = util1 + idle1 + freq1 + entry1;

                            Console.WriteLine(t+" u=" + util1 + " f=" + freq1 + " it=" + idle1 + " ie=" + entry1 + " total="+total + " power="+power);

                            utilList.Add(util1);
                            idleList.Add(idle1);
                            freqList.Add(freq1);
                            entryList.Add(entry1);
                            powerList.Add(power);
                            compare[t] = total;

                            accUtil.Clear();
                            accIdleTime.Clear();
                            accFreq.Clear();
                            accEntryData.Clear();
                        }

                        int max = 0;
                        int min = 0;

                        foreach (KeyValuePair<int, double> data in compare.OrderByDescending(key => key.Value))
                        {
                            Console.WriteLine("Key: {0}, Value: {1}", data.Key, data.Value);
                            max = data.Key-1;
                            break;
                        }

                        foreach (KeyValuePair<int, double> data in compare.OrderBy(key => key.Value))
                        {
                            Console.WriteLine("Key: {0}, Value: {1}", data.Key, data.Value);
                            min = data.Key-1;
                            break;
                        }

                        Console.WriteLine("max");

                       /* double utilSum = 0;
                        double idleSum = 0;
                        double freqSum = 0;
                        double entrySum = 0;

                        for (int i = 0; i < 7; i++)
                        {
                            if (i == (max-1) || i == (min-1)) continue;

                           utilSum += utilList.ElementAt(i);
                           idleSum += idleList.ElementAt(i);
                           freqSum += freqList.ElementAt(i);
                           entrySum += entryList.ElementAt(i); 

                          
                        }

                        double avgUtil = utilSum / 5.0f;
                        double avgIdle = idleSum / 5.0f;
                        double avgFreq = freqSum / 5.0f;
                        double avgEntry = entrySum / 5.0f;

                        Console.WriteLine("Average u=" + avgUtil + " f=" + avgFreq + " it=" + avgIdle + " ie=" + avgEntry); */

                    
                        utilList.RemoveAt(min);
                        idleList.RemoveAt(min);
                        freqList.RemoveAt(min);
                        entryList.RemoveAt(min);
                        powerList.RemoveAt(min);

                        --max;
                        
                        utilList.RemoveAt(max);
                        idleList.RemoveAt(max);
                        freqList.RemoveAt(max);
                        entryList.RemoveAt(max);
                        powerList.RemoveAt(max);

                        Console.WriteLine("util=" + utilList.Median() + " freq=" + freqList.Median() + " idle_time=" + idleList.Median() + " idle_entry=" + entryList.Median() + " power="+powerList.Median());

                        utilList.Clear();
                        idleList.Clear();
                        freqList.Clear();
                        entryList.Clear();
                        powerList.Clear();
                    }
                }
            }
        }
    }
}
