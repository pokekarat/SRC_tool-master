using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TrainDUTs
{
    public class trainCPu
    {
       
        public static void train()
        {
            
            //More details of idle time setting can be found at the paper 
            //Towards better CPU  power management  on multicore  smartphones
            int[] idle = { 1, 10, 50, 100, 500, 1000 }; //idle time
            int[] util = { 1, 35, 60, 100 }; // expect { 10, 50, 75, 100 }

            ArrayList measures = new ArrayList();

            String command = "";

            Console.WriteLine("Start training >> ");
            

            for (int c = 0; c < Config.cpuNums.Length; c++)
            {
                //Setting up frequency
                for (int f = 0; f < Config.freqs.Length; f++)
                {

                    Console.WriteLine("Set min freq = " + Config.freqs[f]);
                    Config.callProcess("echo " + Config.freqs[f] + " > /sys/devices/system/cpu/cpu" + Config.cpuNums[0] + "/cpufreq/scaling_min_freq");

                    Console.WriteLine("Set max freq = " + Config.freqs[f]);
                    Config.callProcess("echo " + Config.freqs[f] + " > /sys/devices/system/cpu/cpu" + Config.cpuNums[0] + "/cpufreq/scaling_max_freq");

                    for (int t = 1; t <= Config.numTests; t++)
                    {
                        //Train util
                        for (int u = 0; u < util.Length; u++)
                        {

                            Console.WriteLine("Test no. " + t + " training... util = " + (util[u]) + " freq = " + Config.freqs[f]);

                            for (int i = 0; i < idle.Length; i++)
                            {
                                //More details of idle time setting can be found at the paper 
                                //Towards better CPU  power management  on multicore  smartphones
                                int y = (util[u] * (idle[i] * 1000)) / (101 - util[u]);
                                int x = (idle[i] * 1000) + y;


                                Console.WriteLine("Call strc " + x + " " + y + " for idle time = " + idle[i] + " (ms)");
                                command = Config.adbPath + "adb shell \"su -c '/data/local/tmp/strc " + x + " " + y + " &'\"";
                                Config.callProcess("/data/local/tmp/strc " + x + " " + y + " &");

                                //strc is the program to vary the cpu utilization.
                                Config.isProcessRunning("strc");

                                //set DUT brightness to low.
                                Console.WriteLine("Set Low DUT brightness 10");
                                Config.callProcess("echo 10 > " + Config.brightPath);

                                //Call Monsoon

                                Config.callPowerMeter(Config.rootPath + "test_" + t + "_freq_" + Config.freqs[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".pt4", 100);

                                Config.checkConnection();

                                //set DUT brightness to high.
                                Console.WriteLine("Set High DUT brightness 100");
                                Config.callProcess("echo 100 > " + Config.brightPath);

                                //kill strc
                                Console.WriteLine("Start kill strc");
                                Config.callProcess("./data/local/tmp/busybox killall strc");
                                Config.isProcessRunning("strc");

                                //pull file
                                Console.WriteLine("Start pull file");
                                string srcFile = "/sdcard/semionline/base.txt";
                                string destFile = Config.rootPath + "test_" + t + "_freq_" + Config.freqs[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".txt";
                                Config.pullFile(srcFile, destFile);
                            }

                            Console.WriteLine("Start charging...");
                            Thread.Sleep(1000 * 60 * 5); // 5 mins break for battery charging.

                        } //util
                    }//num of test
                }//freq
            }//num of cpu
        }
    }
}
