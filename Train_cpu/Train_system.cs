using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Train_cpu
{
    public class Train_system
    {
        String folderPath = @"C:\Users\pok\Semionline\Nexus\CPU_idle\";
        String adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
      
        public Train_system()
        {}

        public void execute()
        {
            // 
            int[] freq = { 400000, 200000 };//, 400000, 800000, 1000000 }; //245760, 320000, 480000, 800000
            int[] idle = { 1, 10, 20, 50, 80, 100, 200, 500, 800, 1000 }; //idle time
            int[] util = {1, 25, 50, 75 }; // { 1, 10, 20, 30, 40, 50, 60, 70, 80, 90, 100};
            ArrayList measures = new ArrayList();

            //TextWriter tw;

           
            String command = "";
          
       
            Console.WriteLine("Start training >> ");

            if (!this.isProcessStart("com.example.trainandroid"))
            {
                //start com.example.trainandroid
                Console.WriteLine("Start com.example.trainandroid");
                command = adbPath + "adb shell \"su -c 'am start -n com.example.trainandroid/com.example.trainandroid.MainActivity --es extraKey start'\"";
                ProcessStartInfo amInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                amInfo.CreateNoWindow = true;
                amInfo.UseShellExecute = false;
                amInfo.RedirectStandardError = true;
                amInfo.RedirectStandardOutput = true;
                Process amProc = Process.Start(amInfo);

                while (!this.isProcessStart("com.example.trainandroid"))
                {
                    Thread.Sleep(5000); //sleep 10 seconds waiting for strc start.
                    Process.Start(amInfo);
                }

            }

        

            Thread.Sleep(30000);

            int cntSrcCall = 0; 
            //Train freq
            for (int f = 0; f < freq.Length; f++)
            {           
            
                command = adbPath + "adb shell \"su -c 'echo " + freq[f] + " > /sys/devices/system/cpu/cpu0/cpufreq/scaling_min_freq'\"";
                ProcessStartInfo cpufreqMin = new ProcessStartInfo("cmd.exe", "/c " + command);
                cpufreqMin.CreateNoWindow = true;
                cpufreqMin.UseShellExecute = false;
                cpufreqMin.RedirectStandardError = true;
                cpufreqMin.RedirectStandardOutput = true;
                Process process1 = Process.Start(cpufreqMin);
                Console.WriteLine("Set min freq = " + freq[f]);
 
                Thread.Sleep(5000);

                command = adbPath + "adb shell \"su -c 'echo " + freq[f] + " > /sys/devices/system/cpu/cpu0/cpufreq/scaling_max_freq'\"";
                ProcessStartInfo cpufreqMax = new ProcessStartInfo("cmd.exe", "/c " + command);
                cpufreqMax.CreateNoWindow = true;
                cpufreqMax.UseShellExecute = false;
                cpufreqMax.RedirectStandardError = true;
                cpufreqMax.RedirectStandardOutput = true;
                Process process2 = Process.Start(cpufreqMax);
                Console.WriteLine("Set max freq = " + freq[f]);

                Thread.Sleep(5000);

                int numOfTest = 7;

                for (int t = 1; t <= numOfTest; t++)
                {
                    //Train util
                    for (int u = 0; u < util.Length; u++)
                    {

                        Console.WriteLine("Test no. "+t+" training... util = " + (util[u]) + " freq = " + freq[f] + "");

                        for (int i = 0; i < idle.Length; i++)
                        {
                            int y = (util[u] * (idle[i] * 1000)) / (101 - util[u]);
                            int x = (idle[i] * 1000) + y;

                            Console.WriteLine("Call strc " + x + " " + y + " for idle time = " + idle[i] + " (ms)");
                            command = adbPath + "adb shell \"su -c '/data/local/tmp/strc " + x + " " + y + " &'\"";
                            ProcessStartInfo cpuInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                            cpuInfo.CreateNoWindow = true;
                            cpuInfo.UseShellExecute = false;
                            cpuInfo.RedirectStandardError = true;
                            cpuInfo.RedirectStandardOutput = true;
                            Process process = Process.Start(cpuInfo);

                            while (!this.isProcessStart("strc"))
                            {
                                Thread.Sleep(10000); //sleep 10 seconds waiting for strc start
                                Process.Start(cpuInfo);

                                ++cntSrcCall;

                                if (cntSrcCall > 10)
                                {
                                    this.callNkillPowerMeter();

                                    cntSrcCall = 0;
                                }
                            }

                            //set DUT brightness to low.
                            Console.WriteLine("Low DUT brightness");
                            command = adbPath + "adb shell \"su -c 'echo 0 > /sys/class/backlight/s5p_bl/brightness'\"";
                            ProcessStartInfo brightInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                            brightInfo.CreateNoWindow = true;
                            brightInfo.UseShellExecute = false;
                            brightInfo.RedirectStandardError = true;
                            brightInfo.RedirectStandardOutput = true;
                            Process brightProc = Process.Start(brightInfo);

                            Thread.Sleep(5000);

                            //Call Monsoon
                            callPowerMeter(folderPath + "test_"+t+"_freq_" + freq[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".pt4");

                            Thread.Sleep(5000);

                            //set DUT brightness to high.
                            Console.WriteLine("High DUT brightness");
                            command = adbPath + "adb shell \"su -c 'echo 255 > /sys/class/backlight/s5p_bl/brightness'\"";
                            brightInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                            Process.Start(brightInfo);

                            Thread.Sleep(5000);

                            //kill strc
                            Console.WriteLine("Start kill strc");
                            string exec1 = adbPath + "adb shell \"su -c 'killall strc'\"";
                            ProcessStartInfo cpuInfo1 = new ProcessStartInfo("cmd.exe", "/c " + exec1);
                            cpuInfo1.CreateNoWindow = true;
                            cpuInfo1.UseShellExecute = false;
                            cpuInfo1.RedirectStandardError = true;
                            cpuInfo1.RedirectStandardOutput = true;
                            Process processKill = Process.Start(cpuInfo1);

                            //while (this.isProcessStart("com.example.trainandroid"))
                            {
                                Thread.Sleep(10000); //sleep 10 seconds waiting for strc start.
                            }

                            //pull file
                            Console.WriteLine("Start pull file");
                            string dataPath = folderPath + "test_" + t + "_freq_" + freq[f] + "_util_" + (util[u]) + "_" + idle[i] + ".txt";
                            command = adbPath + "adb pull /sdcard/semionline/base.txt " + dataPath;
                            ProcessStartInfo pullInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                            pullInfo.CreateNoWindow = true;
                            pullInfo.UseShellExecute = false;
                            pullInfo.RedirectStandardError = true;
                            pullInfo.RedirectStandardOutput = true;
                            Process pullProc = Process.Start(pullInfo);

                            Thread.Sleep(10000);

                        }

                        Console.WriteLine("Start charging...");
                        Thread.Sleep(1000 * 60 * 5); // 5 mins break for battery charging.

                        /* for (int i = 1; i <= 5; i++)
                         {
                             Console.Write(" " + i);
                             string powerPath = folderPath + @"\freq_" + freq[f] + "_util_" + u + "_" + i + ".pt4";
                             //tw = new StreamWriter(powerPath);

                             //Call Monsoon.
                             Process powerMonitor = new Process();
                             powerMonitor.StartInfo.FileName = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";
                             powerMonitor.StartInfo.Arguments = "/USBPASSTHROUGH=AUTO /VOUT=4.20 /KEEPPOWER /NOEXITWAIT /SAVEFILE=" +powerPath+ "  /TRIGGER=DTXD100A"; //DTYD60A
                             powerMonitor.Start();
                             powerMonitor.WaitForExit();
                             Thread.Sleep(delayAfterMonsoon);

                             Console.WriteLine("End loops");
                         }

                         Thread.Sleep(5000);
                         */

                    }
                }

            }
                       
        }

        public void callPowerMeter(String savePath)
        {
            Console.WriteLine("Start monsoon");
            String pathPowerSave = savePath;
           
            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";
            powerMonitor.StartInfo.Arguments = "/USBPASSTHROUGH=AUTO /VOUT=4.20 /KEEPPOWER /NOEXITWAIT /SAVEFILE=" + pathPowerSave + "  /TRIGGER=DTXD50A"; //DTYD60A
            powerMonitor.Start();
            powerMonitor.WaitForExit();
        }

        public void callNkillPowerMeter()
        {

            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";
            powerMonitor.Start();

            Thread.Sleep(5000);

            powerMonitor.Kill();

            Thread.Sleep(5000);
        }


        public Boolean isProcessStart(string name)
        {
            String adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
            string command = adbPath + "adb shell ps | grep "+name;
            ProcessStartInfo cpuInfo1 = new ProcessStartInfo("cmd.exe", "/c " + command);
            cpuInfo1.CreateNoWindow = true;
            cpuInfo1.UseShellExecute = false;
            cpuInfo1.RedirectStandardError = true;
            cpuInfo1.RedirectStandardOutput = true;
            Process process = Process.Start(cpuInfo1);

            string output = process.StandardOutput.ReadToEnd();
            string error = process.StandardError.ReadToEnd();
            process.Close();

            Console.WriteLine("Output = " + output);

            if (output.Length > 0)
                return true;
            else
                return false;

           
        }

        public void checkBattStatus(string path)
        {

            //check battery
            ProcessStartInfo battInfo = new ProcessStartInfo("cmd.exe", "/c " + "echo sh -c \" cat /sys/class/power_supply/battery/capacity > /data/local/tmp/batt_cap.txt \" | adb shell");
            battInfo.CreateNoWindow = true;
            battInfo.UseShellExecute = false;
            battInfo.RedirectStandardError = true;
            battInfo.RedirectStandardOutput = true;
            Process battProcess = Process.Start(battInfo);
            Thread.Sleep(2000);
            if (!battProcess.HasExited)
                battProcess.Kill();

            ProcessStartInfo battpullInfo = new ProcessStartInfo("cmd.exe", "/c " + "adb pull /data/local/tmp/batt_cap.txt "+path+"\\batt.txt");
            battpullInfo.CreateNoWindow = true;
            battpullInfo.UseShellExecute = false;
            battpullInfo.RedirectStandardError = true;
            battpullInfo.RedirectStandardOutput = true;
            Process battpullProcess = Process.Start(battpullInfo);
            Thread.Sleep(2000);
            if (!battpullProcess.HasExited)
                battpullProcess.Kill();

            int batt_cap = 0;
            if (File.Exists(path + "\\batt.txt"))
            {
                var lines = File.ReadAllLines(path + "\\batt.txt");
                foreach (var line in lines)
                {
                    batt_cap = int.Parse(line);
                }
            }

            //if (batt_cap <= 50)
            int chargeCap = 100 - batt_cap;

            Console.WriteLine("Charing");
            Thread.Sleep(chargeCap * 30 * 1000); //0.5 min per % 
            
            Console.WriteLine("Finish charging");
        }
    }
}
