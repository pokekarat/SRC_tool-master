using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Train_DUT
{
  
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

        public static void trainS4cpu()
        {

            System.Media.SystemSounds.Asterisk.Play();
            
            int numTest = 1;

            string[] freqs = { /*"250000", "350000", "450000", "500000", "550000", "600000",*/ "800000" , "900000", "1000000", "1100000", "1200000", "1300000", "1400000", "1500000", "1600000" };
            int[] utils = { 25 , 60 };
            int[] idleTimes = { 20 , 100, 500, 1000 };

            int[] numCoreEnable = { 1, 2, 3, 4 };

            int index = 1;

            for (int f = 0; f < freqs.Length; f++)
            {
                for (int u = 0; u < utils.Length; u++)
                {
                    for (int it = 0; it < idleTimes.Length; it++)
                    {
                        for (int c = 0; c < numCoreEnable.Length; c++)
                        {

                            Config.checkConnection();

                            string freqActive = freqs[f];
                            int utilActive = utils[u];
                            int idleTime = idleTimes[it];
                            int numCoreActive = numCoreEnable[c];

                            //Set cores
                            if (numCoreActive == 1)
                            {
                                Config.callProcess("echo 0 > /sys/devices/system/cpu/cpu1/online");
                                Config.callProcess("echo 0 > /sys/devices/system/cpu/cpu2/online");
                                Config.callProcess("echo 0 > /sys/devices/system/cpu/cpu3/online");
                            }
                            else if (numCoreActive == 2)
                            {
                                Config.callProcess("echo 1 > /sys/devices/system/cpu/cpu1/online");
                                Config.callProcess("echo 0 > /sys/devices/system/cpu/cpu2/online");
                                Config.callProcess("echo 0 > /sys/devices/system/cpu/cpu3/online");
                            }
                            else if (numCoreActive == 3)
                            {
                                Config.callProcess("echo 1 > /sys/devices/system/cpu/cpu1/online");
                                Config.callProcess("echo 1 > /sys/devices/system/cpu/cpu2/online");
                                Config.callProcess("echo 0 > /sys/devices/system/cpu/cpu3/online");
                            }
                            else if (numCoreActive == 4)
                            {
                                Config.callProcess("echo 1 > /sys/devices/system/cpu/cpu1/online");
                                Config.callProcess("echo 1 > /sys/devices/system/cpu/cpu2/online");
                                Config.callProcess("echo 1 > /sys/devices/system/cpu/cpu3/online");
                            }

                            int y = (utils[u] * (idleTimes[it] * 1000)) / (101 - utils[u]);
                            int x = (idleTimes[it] * 1000) + y;

                            for (int nc = 1; nc <= numCoreActive; nc++)
                            {

                                Config.callProcess("./data/local/tmp/strc " + x + " " + y + " &");
                            }

                            //Set freq
                            for (int nc = 0; nc < numCoreActive; nc++)
                            {
                                Config.callProcess("echo " + freqActive + " > /sys/devices/system/cpu/cpu" + nc + "/cpufreq/scaling_min_freq");
                                Config.callProcess("echo " + freqActive + " > /sys/devices/system/cpu/cpu" + nc + "/cpufreq/scaling_max_freq");
                            }

                            string saveFolder = Config.rootPath + "CPU";

                            if (!Directory.Exists(saveFolder))
                                Directory.CreateDirectory(saveFolder);

                            for (int i = 1; i <= numTest; i++)
                            {

                                new Thread(new ThreadStart(Config.Run)).Start();

                                Config.callProcess("./data/local/tmp/OGLES2PVRScopeExampleS4 " + index + " " + Config.time + " &");

                                Thread.Sleep(10000);

                                Config.callPowerMeter(saveFolder + @"\t" + index + "_f" + freqActive + "_u" + utilActive + "_c" + numCoreActive + "_idle_" + idleTime + "_" + i + ".pt4", Config.time);

                            }

                            Thread.Sleep(10000);
                            
                            Config.checkConnection();

                            Config.callProcess("./data/local/tmp/busybox killall strc");

                            Thread.Sleep(5000);

                            Config.callProcess("chmod 777 data/local/tmp/stat/sample" + index + ".txt");

                            Thread.Sleep(5000);

                            Config.callProcess2("pull data/local/tmp/stat/sample" + index + ".txt "+Config.rootPath+"CPU");
                            
                            Thread.Sleep(10000);
                            ++index;
                        }
                    }
                    
                }
            }
        }
    }
}
    
    

