using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Train_DUT
{
    public class evalGPU
    {
        string savePath = @"G:\SemiOnline\Experiment\Nexus\GPU";

        public evalGPU()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

           
        }

        public void Measure( )
        {
            Config.callProcess("chmod 777 /sys/class/backlight/s5p_bl/brightness");

            Thread.Sleep(5000);

            int numTest = 1;

            for (int i = 1; i <= numTest; i++)
            {
                Config.callProcess("./data/local/tmp/OGLES2PVRScopeExample "+i+" &");

                Config.callPowerMeter(savePath + @"\power"+i+".pt4",140);

                Thread.Sleep(10000);

                //Config.pullFile("data/local/tmp/stat/sample" + i + ".txt", savePath);

                //Thread.Sleep(10000);
            }

            Thread.Sleep(30000);

            Config.pullFile("data/local/tmp/stat/", savePath);

            Thread.Sleep(5000);
        }

        public void Evaluate()
        {
            
            List<List<string>> lists = new List<List<string>>();
            List<string> list = null;
            ArrayList saveData = new ArrayList();
            ArrayList saveGPU = new ArrayList();
            
            for (int i = 1; i <= 1; i++)
            {


                string[] datas = File.ReadAllLines(savePath + @"\sample" + i + ".txt");
                double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                for (int j = 0; j < datas.Length; j++)
                {

                    if (datas[j] == "")
                    {

                        if (list != null)
                        {
                            lists.Add(list);
                        }

                        list = new List<string>();
                        continue;

                    }

                    string[] dats = datas[j].Split('=');

                    if (dats[0].Contains("cpu"))
                    {
                        string[] str = dats[0].Split('_');
                        list.Add(str[0]);
                    }

                    Console.WriteLine(j);
                    list.Add(dats[1]);

                }

                lists.Add(list);
                list = null;

                Console.WriteLine("Process each line");

                int row = lists.Count;
                int col = lists[0].Count;
                string values = "";
                string values2 = "";

                saveData.Add("util freq idle_time idle_usage bright ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power");
                saveGPU.Add("ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power");
                
                for (int r = 4; r < row; r++)
                {
                    List<string> curData = lists[r];

                    double util = 0;
                    double freq = 0;
                    double tc = 0;
                    double ec = 0;
                    double bright = 0;
                    for (int c = 0; c < col; c++)
                    {
                        string data = curData[c];
                        
                        double value = double.Parse(data);
                        values += value + " ";

                        if (c == 0) util = value;
                        else if (c == 1) freq = value;
                        else if (c == 2) tc = value;
                        else if (c == 3) ec = value;
                        else if (c==4) bright = value;

                        if (c > 4)
                            values2 += value + " ";
                    }

                    if (ec == 0) ec = 1;

                    double estModel = Config.estCpuPower(util, freq, tc, ec) + Config.estDisplayPower(bright);

                    if (r + 1 == powers.Length)
                    {
                        values += powers[r];

                        if (powers[r] - estModel < 0)
                            values2 += "1";
                        else 
                            values2 += powers[r] - estModel;
                    }
                    else
                    {
                        values += powers[r + 1];

                        if (powers[r] - estModel < 0)
                            values2 += "1";
                        else 
                            values2 += powers[r] - estModel;
                    }

                    saveData.Add(values);
                    saveGPU.Add(values2);
                    
                    values = "";
                    values2 = "";
                }

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                File.WriteAllLines(this.savePath + @"\raw_data_"+i+".txt", toSave);
                saveData.Clear();


                string[] toSave2 = (string[])saveGPU.ToArray(typeof(string));
                File.WriteAllLines(this.savePath + @"\gpu_power_" + i + ".txt", toSave2);
                saveGPU.Clear();

            }
        }
    }
}
