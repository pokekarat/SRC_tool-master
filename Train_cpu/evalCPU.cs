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

    /*
    public class evalCPU
    {
        string rootPath = @"D:\SemiOnline\Experiment\Nexus\CPU_idle";
       
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

        private ArrayList trainSet = new ArrayList();
        private ArrayList testSet = new ArrayList();

        private ArrayList resultNotFound = new ArrayList();

        public evalCPU()
        {
           
        }

        public void execute()
        {

          
            string[] freqs = { "1000000" }; //"200000" , "400000", "800000" };
            string[] utils = { "1", "25", "50", "75" };
            string[] idles = {"1","10","20" ,"50","80","100","200","500","800","1000"};

            trainSet.Add("freq util idleTime idleEntry power");
            testSet.Add("freq util idleTime idleEntry power");


            for (int f = 0; f < freqs.Length; f++)
            {
                for (int u = 0; u < utils.Length; u++)
                {
                    for (int d = 0; d < idles.Length; d++)
                    {
                        
                        Console.WriteLine("\nFile = freq_" + freqs[f] + "_util_" + utils[u] + "_" + idles[d] + ".txt");

                        for (int t = 1; t <= 7; t++)
                        {
                            string dataFile = @"\test_" + t + "_freq_" + freqs[f] + "_util_" + utils[u]+"_"+idles[d]+".txt";
                            string powerFile = @"\test_" + t + "_freq_" + freqs[f] + "_util_" + utils[u] + "_idle_" + idles[d] + ".pt4";

                            double power = Tool.powerParse(samplePath + powerFile, 10);

                            if (!File.Exists(samplePath + dataFile))
                            {
                                resultNotFound.Add(samplePath + dataFile);
                                continue;
                            }

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

                        if (utilList.Count != 7)
                        {
                            utilList.Add(utilList.Median());
                            idleList.Add(idleList.Median());
                            freqList.Add(freqList.Median());
                            entryList.Add(entryList.Median());
                            powerList.Add(powerList.Median());
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

                        compare.Clear();
                      
                        if (min < 0)
                        {
                            min = 0;
                        }
                        else if (min >= utilList.Count)
                        {
                            min = utilList.Count - 1;
                        }

                        if (max < 0)
                        {
                            max = 0;
                        }
                        else if (max >= utilList.Count)
                        {
                            max = utilList.Count - 1;
                        }

                        if (min < max)
                        {
                            utilList.RemoveAt(max);
                            idleList.RemoveAt(max);
                            freqList.RemoveAt(max);
                            entryList.RemoveAt(max);
                            powerList.RemoveAt(max);

                            utilList.RemoveAt(min);
                            idleList.RemoveAt(min);
                            freqList.RemoveAt(min);
                            entryList.RemoveAt(min);
                            powerList.RemoveAt(min);
                        }
                        else
                        {
                            utilList.RemoveAt(min);
                            idleList.RemoveAt(min);
                            freqList.RemoveAt(min);
                            entryList.RemoveAt(min);
                            powerList.RemoveAt(min);

                            utilList.RemoveAt(max);
                            idleList.RemoveAt(max);
                            freqList.RemoveAt(max);
                            entryList.RemoveAt(max);
                            powerList.RemoveAt(max);
                        }

                        string output = "test_idle="+idles[d]+" freq=" + freqList.Median() + " util=" + utilList.Median() + " idle_time=" + idleList.Median() + " idle_entry=" + entryList.Median() + " power=" + powerList.Median();

                        ArrayList rank = new ArrayList();
                        rank.Add(0);
                        rank.Add(1);
                        rank.Add(2);
                        rank.Add(3);
                        rank.Add(4);

                        List<int> generated = new List<int>();
                        while (rank.Count != 0)
                        {
                            Random ran = new Random();
                            int data =  ran.Next(0, rank.Count-1);
                            generated.Add((int)rank[data]);
                            rank.RemoveAt(data);
                        }

                        for (int i = 0; i < generated.Count; i++)
                        {
                            double freq = freqList[i];
                            double util = utilList[i];
                            double idleTime = idleList[i];
                            double idleEntry = entryList[i];
                            double power = powerList[i];

                            //i<2 for train data
                            if (i < 2)
                            {
                               
                                trainSet.Add(freq + " " + util + " " + idleTime + " " + idleEntry + " " + power);
                            }
                            else
                            {
                                testSet.Add(freq + " " + util + " " + idleTime + " " + idleEntry + " " + power);
                            }
                        }
                     
                        utilList.Clear();
                        idleList.Clear();
                        freqList.Clear();
                        entryList.Clear();
                        powerList.Clear();
                    }
                }
            }

            string[] trainData = (string[])trainSet.ToArray(typeof(string));
            File.WriteAllLines(samplePath + @"\output\trainSet_1GHZ.txt", trainData);

            string[] testData = (string[])testSet.ToArray(typeof(string));
            File.WriteAllLines(samplePath + @"\output\testSet_1GHZ.txt", testData);

            string[] resultNotFoundStr = (string[])resultNotFound.ToArray(typeof(string));
            File.WriteAllLines(samplePath + @"\output\resultNotFound_1GHZ.txt", resultNotFoundStr);
            
        }
    }
    */


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

                        double[] powers = Tool.powerParseArr(powerFiles[1], 0, 5000);

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
    
    

