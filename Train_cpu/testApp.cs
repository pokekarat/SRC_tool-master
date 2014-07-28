using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Train_DUT
{
    public class testApp
    {
       
        public static void Evaluate()
        {
            string savePath = @"G:\SemiOnline\Experiment\Nexus\Real_Test\app2";

            List<List<string>> lists = new List<List<string>>();

           
            ArrayList saveData = new ArrayList();

            for (int i = 1; i <= 1; i++)
            {

                string[] datas = File.ReadAllLines(savePath + @"\sample" + i + ".txt");
                double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                lists = Config.processData(datas);

                int row = lists.Count;
                int col = lists[0].Count;
                string values = "";
               
                //saveData.Add("util freq idle_time idle_usage bright tx rx ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power");
                List<double> modelValue = new List<double>();
                List<double> measureValue = new List<double>();

                for (int r = 4; r < row; r++)
                {
                    List<string> curData = lists[r];

                    double util = 0;
                    double freq = 0;
                    double tc = 0;
                    double ec = 0;
                    double bright = 0;
                    double fps = 0;
                    double g3d_core = 0;
                    double tul = 0;
                    double usse_ld_ver = 0;

                    for (int c = 0; c < col; c++)
                    {
                        string data = curData[c];

                        double value = double.Parse(data);
                        values += value + " ";

                        if (c == 0) util = value;
                        else if (c == 1) freq = value;
                        else if (c == 2) tc = value;
                        else if (c == 3) ec = value;
                        else if (c == 4) bright = value;
                        else if (c == 8) fps = value;
                        else if (c == 9) g3d_core = value;
                        else if (c == 14) tul = value;
                        else if (c == 18) usse_ld_ver = value;

                    }

                    //Console.WriteLine("\nutil = " + util + " freq=" + freq + " tc=" + tc + " ec=" + ec + " bright=" + bright + " fps=" + fps + " 3d core=" + g3d_core + " TUL=" + tul + " usse_ld_ver=" + usse_ld_ver);

                    double cpuPower = Config.estCpuPower(util, freq, tc, ec); //Console.WriteLine("cpu pw = " + cpuPower);
                    double disPower = Config.estDisplayPower(bright); //Console.WriteLine("dis pw = " + disPower);
                    double gpuPower = Config.estGpuPower(fps, g3d_core, tul, usse_ld_ver); //Console.WriteLine("gpu pw = " + gpuPower);

                    double estPower = cpuPower + disPower + gpuPower;

                    double measurePower = 0;

                    if (r + 2 < powers.Length)
                    {
                        measurePower = powers[r + 2];
                    }

                    if (measurePower > 0)
                    {
                        modelValue.Add(estPower);
                        measureValue.Add(measurePower);
                    }
                   
                }

                double percentErr = Config.MAPE(measureValue, modelValue);
                Console.WriteLine("test "+i+" has error = " + Math.Round(percentErr,2) + " %");

                string toCVS = "measure;estimate\n";
                for (int j = 0; j < measureValue.Count; j++)
                {
                    string var = measureValue[j] + ";" + modelValue[j] + "\n";
                    toCVS += var;
                }

                File.WriteAllText(savePath + @"\angry_cvs"+i+".cvs", toCVS);
               
               /* string[] toSave2 = (string[])saveData.ToArray(typeof(string));
                File.WriteAllLines(savePath + @"\error" + i + ".txt", toSave2);
                saveData.Clear();*/

                modelValue.Clear();
                measureValue.Clear();

            }
        }
    }
}
