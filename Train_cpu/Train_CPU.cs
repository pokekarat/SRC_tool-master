using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.IO;
using System.Diagnostics;
using System.Threading;

namespace Train_DUT
{
    public class Train_CPU
    {
        String rootPath = "";
        String folderPath = "";
        String adbPath = "";
        String trainAppName = "";
        String brightPath = "";
        String powerMeterPath = "";

        int[] freqs;
        int[] cpuNums;
        
        public Train_CPU(int phone_id)
        {
            //Nexus S
            if (phone_id == 1)
            {
                rootPath = @"C:\Users\pok\Semionline\";
                folderPath = rootPath + @"Nexus\CPU_idle\";
                adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
                trainAppName = "com.example.trainandroid";
                brightPath = "/sys/class/backlight/s5p_bl/brightness";
                powerMeterPath = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";

                freqs = new int[]{ 100000, 200000, 400000, 800000, 1000000 };
                cpuNums = new int[] { 0 };
               
            }
            //Galaxy S4
            else if (phone_id == 2)
            {
                rootPath = @"C:\Users\pok\Semionline\"; //@"D:\Semionline\";
                folderPath = rootPath + @"S4\CPU_idle\";
                adbPath = @"C:\Users\pok\android\sdk\platform-tools\"; // @"D:\android-sdk_r16-windows\android-sdk-windows\platform-tools\";
                trainAppName = "com.example.trains4";
                brightPath = "/sys/class/backlight/panel/brightness";
                powerMeterPath = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd"; //@"D:\Program Files (x86)\Monsoon Solutions Inc\Power Monitor";

                //freqs = new int[] { 250000, 300000, 350000, 450000, 500000, 550000, 600000, 800000, 900000, 1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 1600000 };
                freqs = new int[] { /*500000, 600000, */ 800000, 900000, 1000000, 1100000, 1200000, 1300000, 1400000, 1500000, 1600000 };
                cpuNums = new int[] { 0,1,2,3 };
                
            }
        
        }

        public void execute()
        {
            int numOfTest = 1;

            int[] idle = { 1, 10, 50, 100, 500, 1000 }; //idle time
            int[] util = { /*1,*/ 35, 60, 100 }; // expect { 10, 50, 75, 100 }

            ArrayList measures = new ArrayList();

            String command = "";

            Console.WriteLine("Start training >> ");

            //Call train app (Android)
            if (!this.isProcessRunning(trainAppName))
            {
                //start com.example.trainandroid
                Console.WriteLine("Start " + trainAppName);
                command = adbPath + "adb shell \"su -c 'am start -n " + trainAppName + "/" + trainAppName + ".MainActivity --es extraKey start'\"";
                ProcessStartInfo amInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                amInfo.CreateNoWindow = true;
                amInfo.UseShellExecute = false;
                amInfo.RedirectStandardError = true;
                amInfo.RedirectStandardOutput = true;
                Process amProc = Process.Start(amInfo);

                while (!this.isProcessRunning(trainAppName))
                {
                    this.runAndKillPowerMeter();
                    Thread.Sleep(5000); //sleep 10 seconds waiting for strc start.
                    Process.Start(amInfo);
                }

            }

            Thread.Sleep(30000);

            int cntSrcCall = 0;

            for (int c = 0; c < 1; /*cpuNums.Length;*/ c++)
            {
                //Train freq
                for (int f = 0; f < freqs.Length; f++)
                {

                    command = adbPath + "adb shell \"su -c 'echo " + freqs[f] + " > /sys/devices/system/cpu/cpu" + cpuNums[0] + "/cpufreq/scaling_min_freq'\"";
                    ProcessStartInfo cpufreqMin = new ProcessStartInfo("cmd.exe", "/c " + command);
                    cpufreqMin.CreateNoWindow = true;
                    cpufreqMin.UseShellExecute = false;
                    cpufreqMin.RedirectStandardError = true;
                    cpufreqMin.RedirectStandardOutput = true;
                    Process process1 = Process.Start(cpufreqMin);
                    Console.WriteLine("Set min freq = " + freqs[f]);

                    Thread.Sleep(5000);

                    command = adbPath + "adb shell \"su -c 'echo " + freqs[f] + " > /sys/devices/system/cpu/cpu" + cpuNums[0] + "/cpufreq/scaling_max_freq'\"";
                    ProcessStartInfo cpufreqMax = new ProcessStartInfo("cmd.exe", "/c " + command);
                    cpufreqMax.CreateNoWindow = true;
                    cpufreqMax.UseShellExecute = false;
                    cpufreqMax.RedirectStandardError = true;
                    cpufreqMax.RedirectStandardOutput = true;
                    Process process2 = Process.Start(cpufreqMax);
                    Console.WriteLine("Set max freq = " + freqs[f]);

                    Thread.Sleep(5000);

                    for (int t = 1; t <= numOfTest; t++)
                    {
                        //Train util
                        for (int u = 0; u < util.Length; u++)
                        {

                            Console.WriteLine("Test no. " + t + " training... util = " + (util[u]) + " freq = " + freqs[f]);

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

                                while (!this.isProcessRunning("strc"))
                                {
                                    Thread.Sleep(10000); //sleep 10 seconds waiting for strc start
                                    Process.Start(cpuInfo);

                                    ++cntSrcCall;

                                    if (cntSrcCall > 10)
                                    {
                                        this.runAndKillPowerMeter();
                                        cntSrcCall = 0;
                                    }
                                }

                                //set DUT brightness to low.
                                Console.WriteLine("Set Low DUT brightness 10");
                                command = adbPath + "adb shell \"su -c 'echo 10 > " + this.brightPath + "'\"";
                                ProcessStartInfo brightInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                                brightInfo.CreateNoWindow = true;
                                brightInfo.UseShellExecute = false;
                                brightInfo.RedirectStandardError = true;
                                brightInfo.RedirectStandardOutput = true;
                                Process brightProc = Process.Start(brightInfo);

                                Thread.Sleep(5000);

                                //Call Monsoon
                                callPowerMeter(folderPath + "test_" + t + "_freq_" + freqs[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".pt4");

                                Thread.Sleep(5000);

                                //Usb not on
                                this.runAndKillPowerMeter();
                                
                                //set DUT brightness to high.
                                Console.WriteLine("Set High DUT brightness 100");
                                command = adbPath + "adb shell \"su -c 'echo 100 > " + this.brightPath + "'\"";
                                brightInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
                                Process.Start(brightInfo);

                                Thread.Sleep(5000);

                                this.runAndKillPowerMeter();

                                //kill strc
                                Console.WriteLine("Start kill strc");
                                string exec1 = adbPath + "adb shell \"su -c './data/local/tmp/busybox killall strc'\"";
                                ProcessStartInfo cpuInfo1 = new ProcessStartInfo("cmd.exe", "/c " + exec1);
                                cpuInfo1.CreateNoWindow = true;
                                cpuInfo1.UseShellExecute = false;
                                cpuInfo1.RedirectStandardError = true;
                                cpuInfo1.RedirectStandardOutput = true;
                                Process processKill = Process.Start(cpuInfo1);

                                while (this.isProcessRunning("strc"))
                                {
                                    Console.WriteLine("Cannot kill strc.");
                                    Thread.Sleep(3000); //sleep 10 seconds waiting for strc start.
                                    Process.Start(cpuInfo1);
                                }

                                //pull file
                              
                                string dataPath = folderPath + "test_" + t + "_freq_" + freqs[f] + "_util_" + (util[u]) + "_idle_" + idle[i] + ".txt";
                                Console.WriteLine("Start pull file "+dataPath);
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

                        }
                    }
                }
            }
        }

        public void callPowerMeter(String savePath)
        {
            Console.WriteLine("Start monsoon");
            String pathPowerSave = savePath;
           
            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = powerMeterPath;
            powerMonitor.StartInfo.Arguments = "/USBPASSTHROUGH=AUTO /VOUT=4.20 /KEEPPOWER /NOEXITWAIT /SAVEFILE=" + pathPowerSave + "  /TRIGGER=DTXD120A"; //DTYD60A
            powerMonitor.Start();
            powerMonitor.WaitForExit();
        }

        public void runAndKillPowerMeter()
        {

            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";
            powerMonitor.Start();

            Thread.Sleep(5000);

            powerMonitor.Kill();

            Thread.Sleep(5000);
        }


        public Boolean isProcessRunning(string name)
        {
            
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
