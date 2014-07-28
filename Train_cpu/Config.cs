using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Train_DUT
{
    public class Config
    {
        public static string rootPath = @"D:\SemiOnline\Experiment\Nexus\WiFi\channel_54";
        public static string adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
        public static string powerMeterPath = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";
                                             
        
        public static double NEXUS_CPU_POWER_MODEL(double freq, double cpu)
        {
            
            double estPower = 0;

            if(freq == 1000.0)
                estPower = (-1.131 * 0.26) + (6.047 * cpu) + 468.957;

            return estPower;
        }

        public static void callPowerMeter(String savePath, int time)
        {
            Console.WriteLine("Start monsoon\n");

            String pathPowerSave = savePath;

            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = powerMeterPath;
            powerMonitor.StartInfo.Arguments = "/USBPASSTHROUGH=AUTO /VOUT=4.20 /KEEPPOWER /NOEXITWAIT /SAVEFILE=" + pathPowerSave + "  /TRIGGER=DTXD"+time+"A"; //DTYD60A
            powerMonitor.Start();
            powerMonitor.WaitForExit();

            Console.WriteLine("End monsoon\n");
        }

        public static void pullFile(string phonePath, string hostPath)
        {
            string command = adbPath + "adb pull " + phonePath +" "+ hostPath;
            
            Console.WriteLine("Start pull file " + command+"\n");
            
            ProcessStartInfo pullInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            pullInfo.CreateNoWindow = true;
            pullInfo.UseShellExecute = false;
            pullInfo.RedirectStandardError = true;
            pullInfo.RedirectStandardOutput = true;
            Process pullProc = Process.Start(pullInfo);
            

            Console.WriteLine("Finish pull file");
        }

        public static void runAndKillPowerMeter()
        {

            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = "C:\\Program Files (x86)\\Monsoon Solutions Inc\\PowerMonitor\\PowerToolCmd";
            powerMonitor.Start();

            Thread.Sleep(5000);

            powerMonitor.Kill();

            Thread.Sleep(5000);
        }

        public static void callProcess(string command)
        {
           
            command = adbPath + "adb shell \"su -c '" + command + "' \"";
            Console.WriteLine("Start " + command+"\n");

            ProcessStartInfo pInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            pInfo.CreateNoWindow = true;
            pInfo.UseShellExecute = false;
            pInfo.RedirectStandardError = true;
            pInfo.RedirectStandardOutput = true;
            Process process = Process.Start(pInfo);
        }

        public static Boolean isProcessRunning(string name)
        {

            string command = adbPath + "adb shell ps | grep " + name;
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

        public static void checkBattStatus(string path)
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

            ProcessStartInfo battpullInfo = new ProcessStartInfo("cmd.exe", "/c " + "adb pull /data/local/tmp/batt_cap.txt " + path + "\\batt.txt");
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

        static double prev_total = 0;
        static double prev_idle = 0;

        public static double parseCPU(string cpuData)
        {

           
            double total = 0;

            string[] cpuElements = cpuData.Split(' ');

            for (int i = 1; i < cpuElements.Length; i++)
            {
                total += Double.Parse(cpuElements[i]);
            }

            double idle = Double.Parse(cpuElements[4]);

            double diff_idle = idle - prev_idle;
            double diff_total = total - prev_total;
            double diff_util = (1000 * (diff_total - diff_idle) / diff_total + 5) / 10;

            prev_total = total;
            prev_idle = idle;

            return diff_util;
        }

        public static double estCpuPower(double util, double freq, double idleTime, double idleEntry)
        {
            double cpuPower = 0;

            if (freq == 200000)
            {
                cpuPower = -0.126 * (idleTime / (idleEntry + 0.01)) + (0.723 * util) + 389.239;
            }
            else if (freq == 400000)
            {
                cpuPower = -0.237 * (idleTime / (idleEntry + 0.01)) + (1.602 * util) + 427.271;
            }
            else if (freq == 800000)
            {
                cpuPower = -0.743 * (idleTime / (idleEntry + 0.01)) + (4.094 * util) + 444.281;
            }
            else if (freq == 1000000)
            {
                cpuPower = -1.131 * (idleTime / (idleEntry + 0.01)) + (6.047 * util) + 468.957;
            }
            else if (freq == 100000)
            {
                cpuPower = (0.2189 * util) + 333.44;
            }
            

            return cpuPower;
        }

        public static double estDisplayPower(double bright)
        {
            return (2.317 * bright) + 0.936;
        }

        public static double estGpuPower(double fps, double t3d_core, double txt_u_l, double usse_ld_ver)
        {
            return (-6.8 * fps) + (-13.4 * t3d_core) + (312.8 * txt_u_l) + (7548.4 * usse_ld_ver) + 71.7;
        }

        public static List<List<string>> processData(string[] d) {

             string[] datas = d;
            
             List<List<string>> lists = new List<List<string>>();
             List<string> list = null;

             for (int j = 0; j < datas.Length; j++)
             {
                if (datas[j] == "") continue;

                if (datas[j].Contains("loop"))
                {
                    if (list != null)
                        lists.Add(list);

                    list = new List<string>();
                    continue;
                }

                if (!datas[j].Contains("="))
                {
                    continue;
                }

                string[] dats = datas[j].Split('=');
                list.Add(dats[1]);
             }

            //Add last list
            lists.Add(list);
            list = null;
            return lists;
         }

        public static double MAPE(List<double> measure, List<double> model)
        {
            double result = 0;

            double acc = 0;
            int dataSize = measure.Count;
            for (int i = 0; i < dataSize-1; i++)
            {
                if (measure[i] == 0) measure[i] = 0.1;

               double sum = Math.Abs(measure[i] - model[i])/measure[i];

               if (sum > 1) continue;

               //Console.WriteLine("sum = " + sum);
               acc += sum;
            }

            result = (acc / dataSize) * 100;

            return result;

        }

        public static void Run()
        {
            for (int i = 1; i <= 100; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(1000);
            }
        }

        public static void measure()
        {
            string savePath = @"G:\SemiOnline\Experiment\Nexus\Real_Test\app4";

            Config.callProcess("rm /data/local/tmp/stat/*.txt");
            Config.callProcess("chmod 777 /sys/class/backlight/s5p_bl/brightness");

            Thread.Sleep(5000);

            int numTest = 7;

            int time = 180;

            for (int i = 1; i <= numTest; i++)
            {

                //if (i == 1)
                {
                    new Thread(new ThreadStart(Run)).Start();
                }

                Config.callProcess("./data/local/tmp/OGLES2PVRScopeExample " + i + " " + time + " &");

                Config.callPowerMeter(savePath + @"\power" + i + ".pt4", time);

                Thread.Sleep(30000);

                //Config.pullFile("data/local/tmp/stat/sample" + i + ".txt", savePath);

                //Thread.Sleep(10000);
            }

            Thread.Sleep(30000);

            Config.pullFile("data/local/tmp/stat/", savePath);

            Thread.Sleep(5000);
        }

        
    }
}
