﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainDUTs
{
    public class trainCPu
    {
      
        public static void train()
        {
            
            //More details of idle time setting can be found at the paper 
            //Towards better CPU  power management  on multicore  smartphones
            int[] idle = { 1 , 10, 50, 100, 500, 1000 }; //idle time
            int[] util = { 1 , 35, 60, 100 }; // expect { 10, 50, 75, 100 }

            ArrayList measures = new ArrayList();

            Console.WriteLine("Start training >> ");
            Tool.showStatus("Start training");

            //set DUT brightness to high.
            Console.WriteLine("Set High DUT brightness 100");
            Config.callProcess("echo 255 > " + Config.brightPath);
            Tool.showStatus("Set bright 255");

            int fileCount = 1;

            for (int t = 1; t <= Config.numTests; t++)
            {
                //Setting up frequency
                //for (int c = 0; c < Config.cpuNums.Length; c++)
                {

                    //Number of test
                    for (int f = 0; f < Config.freqs.Length; f++)
                    {
                        
                        Console.WriteLine("Set min freq = " + Config.freqs[f]);
                        Config.callProcess("echo " + Config.freqs[f] + " > /sys/devices/system/cpu/cpu" + Config.cpuNums[0] + "/cpufreq/scaling_min_freq");
                        Tool.showStatus("Set min freq = " + Config.freqs[f]);

                        Console.WriteLine("Set max freq = " + Config.freqs[f]);
                        Config.callProcess("echo " + Config.freqs[f] + " > /sys/devices/system/cpu/cpu" + Config.cpuNums[0] + "/cpufreq/scaling_max_freq");
                        Tool.showStatus("Set max freq = " + Config.freqs[f]);

                        //Train util
                        for (int u = 0; u < util.Length; u++)
                        {

                            //Train idle
                            for (int i = 0; i < idle.Length; i++)
                            {
                                Console.WriteLine("Test no. " + t + " training... util = " + (util[u]) + " freq = " + Config.freqs[f]);
                                int sampleTime = Config.time + Config.offset;
                                Config.callProcess("/data/local/tmp/sample " + fileCount + " " + sampleTime + " &");
                                Tool.showStatus("Call sample " + fileCount);
                               

                                //More details of idle time setting can be found at the paper 
                                //Towards better CPU  power management  on multicore  smartphones
                                int y = (util[u] * (idle[i] * 1000)) / (101 - util[u]);
                                int x = (idle[i] * 1000) + y;

                                Tool.showStatus("Call strc " + x + " " + y + " for idle time = " + idle[i] + " (ms)");
                                Config.callProcess("/data/local/tmp/strc " + x + " " + y + " &");
                                
                                //strc is the program to vary the cpu utilization.
                                Config.isProcessRunning("strc");

                                //set DUT brightness to low.
                                Tool.showStatus("Set Low DUT brightness 0");
                                Config.callProcess("echo 0 > " + Config.brightPath);

                                //Call Monsoon
                                //Tool.showStatus("Monsoon is measuring");
                                //Config.callPowerMeter(Config.rootPath + "test_" + t + "_freq_" + Config.freqs[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".pt4", Config.time);

                                //Config.checkConnection();
                                //showStatus("Connection passed.");

                                //set DUT brightness to high.
                                Tool.showStatus("Set High DUT brightness 255");
                                Config.callProcess("echo 255 > " + Config.brightPath);

                                //kill strc
                                Tool.showStatus("Start kill strc");
                                Config.callProcess("./data/local/tmp/busybox killall strc");
                                Config.isProcessRunning("strc");

                                Tool.showStatus("Waiting...");
                                Thread.Sleep(((sampleTime+5) - Config.time) * 1000);

                                //pull file                              
                                string srcFile = "/data/local/tmp/stat/sample"+fileCount+@".txt";
                                Tool.showStatus("Start pull file " + srcFile);
                                string destFile = Config.rootPath + "test_" + t + "_freq_" + Config.freqs[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".txt";
                                Config.pullFile(srcFile, destFile);

                                Tool.showStatus("End " + fileCount);
                                ++fileCount;
                            }
                       } //util

                       //Charging for 10 mins
                       Thread.Sleep(10 * 1000 * 60);

                    }//num of test
                }//freq
            }//num of cpu

            Tool.showStatus("Finish");
        }
    }
}
