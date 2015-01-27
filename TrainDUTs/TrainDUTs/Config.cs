using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainDUTs
{
    public class Config
    {
        public static string rootPath = @"G:\Semionline\Experiment\Nexus\bluetooth\";
        public static string adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
        public static string brightPath = "";
        public static string blankPath = "";

        public static string powerMeterPath = @"C:\Program Files (x86)\Monsoon Solutions Inc\PowerMonitor\PowerToolCmd";
        public static int DUT = 1; //0=nexus, 1=S4
        public static int numTests = 3;
        public static int fileIndex;
        public static int duration;
        public static int offset = 20;
        //CPU
        public static int[] freqs;
        public static int[] cpuNums;

        public static void callPowerMeter(String savePath, int time)
        {
            Console.WriteLine("Start monsoon\n");

            Process powerMonitor = new Process();
            powerMonitor.StartInfo.FileName = powerMeterPath;
            powerMonitor.StartInfo.Arguments = "/USBPASSTHROUGH=AUTO /VOUT=4.20 /KEEPPOWER /NOEXITWAIT /SAVEFILE=" + savePath + "  /TRIGGER=DTXD" + time + "A"; //DTYD60A
            powerMonitor.Start();
            powerMonitor.WaitForExit();

            Console.WriteLine("End monsoon\n");
        }

        public static void pullFile(string phonePath, string hostPath)
        {
            string command = adbPath + "adb pull " + phonePath + " " + hostPath;

            Console.WriteLine("Start pull file " + command + "\n");
            ProcessStartInfo pullInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            pullInfo.CreateNoWindow = true;
            pullInfo.UseShellExecute = false;
            pullInfo.RedirectStandardError = true;
            pullInfo.RedirectStandardOutput = true;
            Process pullProc = Process.Start(pullInfo);

            //callProcess(command);
            
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

            command = adbPath + "adb shell \"su -c '" + command + "'\"";
            Console.WriteLine("Start " + command + "\n");

            ProcessStartInfo pInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            pInfo.CreateNoWindow = true;
            pInfo.UseShellExecute = false;
            pInfo.RedirectStandardError = true;
            pInfo.RedirectStandardOutput = true;
            Process process = Process.Start(pInfo);
            StreamReader sOut = process.StandardOutput;
            string result = sOut.ReadLine();
            Thread.Sleep(3000);
        }

        public static void callProcess2(string command)
        {

            command = adbPath + "adb " + command;
            Console.WriteLine("Start " + command + "\n");

            ProcessStartInfo pInfo = new ProcessStartInfo("cmd.exe", "/c " + command);
            pInfo.CreateNoWindow = true;
            pInfo.UseShellExecute = false;
            pInfo.RedirectStandardError = true;
            pInfo.RedirectStandardOutput = true;
            Process process = Process.Start(pInfo);
            StreamReader sOut = process.StandardOutput;
            string result = sOut.ReadLine();

            Console.WriteLine("Output = " + result);

            Thread.Sleep(5000);
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

        public static double parseCPUutil(string cpuData)
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


        public static List<List<string>> processData(string[] d)
        {

            string[] datas = d;

            List<List<string>> lists = new List<List<string>>();
            List<string> list = null;

            int countLoop = 0;
            for (int j = 0; j < datas.Length; j++)
            {
                if (datas[j] == "") continue;

                if (datas[j].Contains("loop"))
                {
                    ++countLoop;
                    //Console.WriteLine("Count loop = "+countLoop);
                    if (list != null)
                    {
                        lists.Add(list);
                    }

                    list = new List<string>();
                    list.Add(datas[j]);
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

        public static List<string> paramName = new List<string>();

        public static double MAPE(List<double> measure, List<double> model)
        {
            double result = 0;

            double acc = 0;
            int dataSize = measure.Count;
            for (int i = 0; i < dataSize - 1; i++)
            {
                if (measure[i] == 0) measure[i] = 0.1;

                double sum = Math.Abs(measure[i] - model[i]) / measure[i];

                if (sum > 1) continue;

                //Console.WriteLine("sum = " + sum);
                acc += sum;
            }

            result = (acc / dataSize) * 100;

            return result;

        }

        public static int time = 30;

        public static void Run()
        {
            for (int i = 1; i <= time; i++)
            {
                Console.WriteLine(i);
                Thread.Sleep(1000);
            }
        }

        public static void measure()
        {
            string savePath = @"G:\SemiOnline\Experiment\S4\GPU";

            Config.callProcess("rm /data/local/tmp/stat/*.txt");

            if (Config.DUT == 0)
                Config.callProcess("chmod 777 /sys/class/backlight/s5p_bl/brightness");
            else
                Config.callProcess("chmod 777 /sys/class/backlight/panel/brightness");


            Thread.Sleep(5000);

            int numTest = 1;

            for (int i = 1; i <= numTest; i++)
            {

                //if (i == 1)
                //{
                new Thread(new ThreadStart(Run)).Start();
                //}


                if (Config.DUT == 0)
                    Config.callProcess("./data/local/tmp/OGLES2PVRScopeExample " + i + " " + time + " &");

                else
                    Config.callProcess("./data/local/tmp/OGLES2PVRScopeExampleS4 " + i + " " + time + " &");

                Config.callPowerMeter(savePath + @"\power" + i + ".pt4", time);

                Thread.Sleep(10000);

                //Config.pullFile("data/local/tmp/stat/sample" + i + ".txt", savePath);

                //Thread.Sleep(10000);
            }


            Thread.Sleep(30000);

            Config.pullFile("data/local/tmp/stat/", savePath);

            Thread.Sleep(5000);
        }

        static string onPath = rootPath+"on.txt";
        public static void checkConnection()
        {
            Config.callProcess2("pull data/local/tmp/on.txt "+rootPath);
            //Config.callProcess("rm /data/local/tmp/stat/*.txt");
            //Thread.Sleep(5000);

            int count = 0;
            //Check wheather on.txt is existing
            while (!File.Exists(onPath))
            {
                Console.WriteLine("Error count = " + count);
                Config.callProcess2("kill-server");
                Config.callProcess2("start-server");
                Config.callProcess2("pull data/local/tmp/on.txt "+rootPath);
                ++count;

                if (count % 10 == 0)
                {

                    Console.Beep(5000, 5000);
                    Console.WriteLine("Have some problem");
                    Config.callPowerMeter(Config.rootPath + "reconnect.pt4", 10);

                }
            }

            File.Delete(onPath);

        }

        public static string SendMail(string toList, string from, string ccList, string subject, string body)
        {

            System.Net.Mail.MailMessage message = new System.Net.Mail.MailMessage();
            System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient();
            string msg = string.Empty;
            try
            {
                System.Net.Mail.MailAddress fromAddress = new System.Net.Mail.MailAddress(from);
                message.From = fromAddress;
                message.To.Add(toList);
                if (ccList != null && ccList != string.Empty)
                    message.CC.Add(ccList);
                message.Subject = subject;
                message.IsBodyHtml = true;
                message.Body = body;
                // We use gmail as our smtp client
                smtpClient.Host = "smtp.gmail.com";
                smtpClient.Port = 587;
                smtpClient.EnableSsl = true;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new System.Net.NetworkCredential(
                    "pokekarat", "MAY25199%");

                smtpClient.Send(message);
                msg = "Successful<BR>";
            }
            catch (Exception ex)
            {
                msg = ex.Message;
            }
            return msg;
        }

        

        // Generate final data of screen
        public static void parseCPUData()
        {

            string[] fileName = { 
                                    "1_idle_1", "1_idle_10","1_idle_50","1_idle_100","1_idle_500","1_idle_1000",
                                    "10_idle_1", "10_idle_10","10_idle_50","10_idle_100","10_idle_500","10_idle_1000",
                                    "25_idle_1", "25_idle_10","25_idle_50","25_idle_100","25_idle_500","25_idle_1000",
                                    "35_idle_1", "35_idle_10","35_idle_50","35_idle_100","35_idle_500","35_idle_1000",
                                    "50_idle_1", "50_idle_10","50_idle_50","50_idle_100","50_idle_500","50_idle_1000",
                                    "60_idle_1", "60_idle_10","60_idle_50","60_idle_100","60_idle_500","60_idle_1000",
                                    "100_idle_1", "100_idle_10","100_idle_50","100_idle_100","100_idle_500","100_idle_1000"
                                };

            ArrayList trainData = new ArrayList();
            ArrayList testData = new ArrayList();

            string header = "util0 c0its0 c0its1 c0its2 c0ies0 c0ies1 c0ies2 freq0 bright power";
            string value = "";

            trainData.Add(header);
            testData.Add(header);

            int fileLen = fileName.Length;

            string freq0 = "1000000";
            string path = "CPU_idle_900_to_1200";

            for (int f = 0; f < fileLen; f++)
            {

                string rootFilePath = @"D:\Research\S4\CPU_old\CPU_one_core\old\" + path + @"\test_1_freq_" + freq0 + "_util_" + fileName[f];

                if (!File.Exists(rootFilePath + @".txt"))
                {
                    //Console.WriteLine("Skip "+rootFilePath + @".txt");
                    continue;
                }

                Console.WriteLine("Exist " + rootFilePath + @".txt");

                string[] data = File.ReadAllLines(rootFilePath + ".txt");

                int begin = 30;
                int dataLen = data.Length;

                double[] powerData = Tool.powerParseArr(rootFilePath + ".pt4", begin, 0, 5000);

                int powLen = powerData.Length;

                int end = Math.Min(dataLen, powLen);

                int len = end;

                int testIndex = len - ((len - begin) / 4);

                for (int i = begin; i < len; i++)
                {

                    string line = data[i];

                    string[] elements = line.Split(' ');

                    if (!rootFilePath.Contains("_100_"))
                    {
                        string util = elements[2].Split('=')[1];
                        if (util.Equals("100.00"))
                        {
                            continue;
                        }

                    }

                    for (int j = 2; j <= 10; j++)
                    {
                        value += elements[j].Split('=')[1] + " ";
                    }

                    value += (powerData[i - begin] - 7.4); //7.4 is the power of (bright = 10)


                    if (i >= testIndex)
                        testData.Add(value);
                    else
                        trainData.Add(value);

                    value = "";

                }
            }

            string[] savetainData = (string[])trainData.ToArray(typeof(string));
            string saveTrainName = @"D:\research\S4\CPU_old\CPU_one_core\output\train_" + freq0 + ".txt";

            string[] savetestData = (string[])testData.ToArray(typeof(string));
            string saveTestName = @"D:\research\S4\CPU_old\CPU_one_core\output\test_" + freq0 + ".txt";

            Console.WriteLine("File save");

            File.WriteAllLines(saveTrainName, savetainData);
            File.WriteAllLines(saveTestName, savetestData);

            trainData.Clear();
            testData.Clear();
        }

       
    }
}
