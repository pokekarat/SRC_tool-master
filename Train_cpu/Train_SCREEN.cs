using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Train_DUT
{
    public class Train_SCREEN
    {
        string samplePath = @"D:\SemiOnline\Experiment\Nexus\CPU_idle";
        private double accFreq;
        private double accIdleTime;
        private double accEntryData;
        private double accUtil;

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
                        for (int t = 1; t <= 7; t++)
                        {
                            string file = @"\test_" + t + "_freq_" + freqs[f] + "_util_" + utils[u]+"_"+idles[d]+".txt";

                            string[] data = File.ReadAllLines(samplePath + file);

                            for (int x = 0; x < data.Length; x++)
                            {
                                if (x == 0 || x == data.Length - 1)
                                {
                                    continue;
                                }

                                string[] param = data[x].Split(' ');

                                for (int y = 0; y < param.Length; y++)
                                {
                                    
                                    if (param[y].Contains("u="))
                                    {

                                        string[] util = param[y].Split('=');
                                        double utilData = Double.Parse(util[1]);
                                        accUtil += utilData;
                                    }
                                    else if (param[y].Contains("f="))
                                    {
                                        string[] freq = param[y].Split('=');
                                        double freqData = Double.Parse(freq[1]);
                                        accFreq += freqData;
                                    }
                                    else if (param[y].Contains("it="))
                                    {
                                        string[] idleTime = param[y].Split('=');
                                        double idleTimeData = Double.Parse(idleTime[1]);
                                        accIdleTime += idleTimeData;
                                    }
                                    else if (param[y].Contains("iu="))
                                    {
                                        string[] entry = param[y].Split('=');
                                        double entryData = Double.Parse(entry[1]);
                                        accEntryData += entryData;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
