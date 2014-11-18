using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace mainApp
{
    public class Config
    {

        public static string rootPath = @"C:\Users\pok\Research\Experiment\Dropbox\Project2_SemiOnline\Experiment\S4\";
        public static string adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
        public static int sampleFileIndex = -1;
        public static string powerMeterPath = @"C:\Program Files (x86)\Monsoon Solutions Inc\PowerMonitor\PowerToolCmd";
        //public static int DUT = 1; //0=nexus, 1=S4
                                             

        public static List<List<string>> processData(string[] d)
        {

            string[] datas = d;

            List<List<string>> lists = new List<List<string>>();
            List<string> list = null;

            int countLoop = 0;
            for (int j = 0; j < datas.Length; j++)
            {
                if (datas[j] == "" || datas[j].Contains("delay") || datas[j].Contains("Start_time")) continue;

                if (datas[j].Contains("_LOOP_"))
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


            Thread.Sleep(3000);
        }

    }
}
