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
        string savePath = @"G:\Semionline\Experiment\S4\GPU\1";

        public evalGPU()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
        }

        public void Evaluate()
        {
            
            List<List<string>> lists = new List<List<string>>();
            
            ArrayList saveData = new ArrayList();
            ArrayList saveGPU = new ArrayList();
            
            for (int i = 1; i <= 1; i++)
            {
                
                string[] datas = File.ReadAllLines(savePath + @"\sample" + i + ".txt");
                double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                lists = Config.processData(datas);

                int row = lists.Count;
                int col = lists[0].Count;
                string values = "";
                string values2 = "";

                saveData.Add("util freq idle_time idle_usage bright tx rx ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power");
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

        public static void extractVideoFrame()
        {
            /*MediaDetector detector = new MediaDetector();
            detector.LoadMedia(@"G:\Semionline\Experiment\S4\GPU\1\SCR_20140801_133651.mp4");
            var src = detector.GetImage(new TimeSpan(TimeSpan.TicksPerSecond));*/

           
           
        }


        public void Evaluate2()
        {

            List<List<string>> lists = new List<List<string>>();

            ArrayList saveData = new ArrayList();
            ArrayList saveGPU = new ArrayList();

            for (int i = 1; i <= 1; i++)
            {

                string[] datas = File.ReadAllLines(savePath + @"\sample" + i + ".txt");
                double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                lists = Config.processData(datas);

                int row = lists.Count;
                int col = lists[0].Count;
                string values = "";
                string values2 = "";

                string gpuVars = "ftime fps gtl2d gtl3d gtlcc gtlta gtt2d gtt3d gttcc gttta spm isp tal usseccpp usseccpv usselp usselv vpf vps power";
                saveData.Add("u0 u1 u2 u3 f0 f1 f2 f3 c0its0 c0its1 c0its2 c1its0 c1its1 c1its2 c2its0 c2its1 c2its2 c3its0 c3its1 c3its2 c0ies0 c0ies1 c0ies2 c1ies0 c1ies1 c1ies2 c2ies0 c2ies1 c2ies2 c3ies0 c3ies1 c3ies2 m br " + gpuVars);
                saveGPU.Add(gpuVars);

                for (int r = 1; r < row; r++)
                {
                    List<string> curData = lists[r];

                    double[] eles = new double[53];

                    double u0 = 0;double u1 = 0;double u2 = 0;double u3 = 0;
                    double f0 = 0;double f1 = 0;double f2 = 0; double f3 = 0;

                    ///

                    double c0its0 = 0;double c0its1 = 0;double c0its2 = 0;

                    double c1its0 = 0;  double c1its1 = 0;                    double c1its2 = 0;

                    double c2its0 = 0;                    double c2its1 = 0;                    double c2its2 = 0;

                    double c3its0 = 0;                    double c3its1 = 0;                    double c3its2 = 0;

                    ///
                    double c0ies0 = 0;                    double c0ies1 = 0;                    double c0ies2 = 0;

                    double c1ies0 = 0;                    double c1ies1 = 0;                    double c1ies2 = 0;

                    double c2ies0 = 0;                    double c2ies1 = 0;                    double c2ies2 = 0;

                    double c3ies0 = 0;                    double c3ies1 = 0;                    double c3ies2 = 0;
                    ///

                    double m = 0;
                    double br = 0;

                    double ftime = 0;                    double fps = 0;                    double gtl2d = 0;
                    double gtl3d = 0;                    double gtlcc = 0;                    double gtlta = 0;
                    double gtt2d = 0;                    double gtt3d = 0;                    double gttcc = 0;
                    double gttta = 0;                    double spm = 0;                    double isp = 0;
                    double tal = 0;                    double usseccpp = 0;                    double usseccpv = 0;
                    double usselp = 0;                    double usselv = 0;                    double vpf = 0;
                    double vps = 0;
                 

                    for (int c = 0; c < col; c++)
                    {
                        string _data = curData[c];

                        if (c == 0) //4 cores util
                        {
                            string[] cpuUtils;
                            cpuUtils = curData[c].Split(' ');
                            u0 = double.Parse(cpuUtils[0]);
                            u1 = double.Parse(cpuUtils[1]);
                            u2 = double.Parse(cpuUtils[2]);
                            u3 = double.Parse(cpuUtils[3]);

                            eles[0] = u0;
                            eles[1] = u1;
                            eles[2] = u2;
                            eles[3] = u3;
                        }
                        else if (c == 3)
                        {
                            f0 = double.Parse(_data);
                            //Console.WriteLine(f0);
                            eles[4] = f0;
                        }
                        else if (c == 4)
                        {
                            f1 = double.Parse(_data);
                            eles[5] = f1;
                        }
                        else if (c == 5)
                        {
                            f2 = double.Parse(_data);
                            eles[6] = f2;
                        }
                        else if (c == 6)
                        {
                            f3 = double.Parse(_data);
                            eles[7] = f3;
                        }
                        else if (c == 7)
                        {
                            c0its0 = double.Parse(_data);
                            eles[8] = double.Parse(_data);
                        }
                        else if (c == 8)
                        {
                            c0its1 = double.Parse(_data);
                            eles[9] = double.Parse(_data);
                        }
                        else if (c == 9)
                        {
                            c0its2 = double.Parse(_data); eles[10] = double.Parse(_data);
                        }
                        else if (c == 10)
                        {
                            c1its0 = double.Parse(_data); eles[11] = double.Parse(_data);
                        }
                        else if (c == 11)
                        {
                            c1its1 = double.Parse(_data); eles[12] = double.Parse(_data);
                        }
                        else if (c == 12)
                        {
                            c1its2 = double.Parse(_data); eles[13] = double.Parse(_data);
                        }
                        else if (c == 13)
                        {
                            c2its0 = double.Parse(_data); eles[14] = double.Parse(_data);
                        }
                        else if (c == 14)
                        {
                            c2its1 = double.Parse(_data); eles[15] = double.Parse(_data);
                        }
                        else if (c == 15)
                        {
                            c2its2 = double.Parse(_data); eles[16] = double.Parse(_data);
                        }
                        else if (c == 16)
                        {
                            c3its0 = double.Parse(_data); eles[17] = double.Parse(_data);
                        }
                        else if (c == 17)
                        {
                            c3its1 = double.Parse(_data); eles[18] = double.Parse(_data);
                        }
                        else if (c == 18)
                        {
                            c3its2 = double.Parse(_data); eles[19] = double.Parse(_data);
                        }
                        else if (c == 19)
                        {
                            c0ies0 = double.Parse(_data); eles[20] = double.Parse(_data);
                        }
                        else if (c == 20)
                        {
                            c0ies1 = double.Parse(_data); eles[21] = double.Parse(_data);
                        }
                        else if (c == 21)
                        {
                            c0ies2 = double.Parse(_data); eles[22] = double.Parse(_data);
                        }
                        else if (c == 22)
                        {
                            c1ies0 = double.Parse(_data); eles[23] = double.Parse(_data);
                        }
                        else if (c == 23)
                        {
                            c1ies1 = double.Parse(_data); eles[24] = double.Parse(_data);
                        }
                        else if (c == 24)
                        {
                            c1ies2 = double.Parse(_data); eles[25] = double.Parse(_data);
                        }
                        else if (c == 25)
                        {
                            c2ies0 = double.Parse(_data); eles[26] = double.Parse(_data);
                        }
                        else if (c == 26)
                        {
                            c2ies1 = double.Parse(_data); eles[27] = double.Parse(_data);
                        }
                        else if (c == 27)
                        {
                            c2ies2 = double.Parse(_data); eles[28] = double.Parse(_data);
                        }
                        else if (c == 28)
                        {
                            c3ies0 = double.Parse(_data); eles[29] = double.Parse(_data);

                        }
                        else if (c == 29)
                        {
                            c3ies1 = double.Parse(_data); eles[30] = double.Parse(_data);
                        }
                        else if (c == 30)
                        {
                            c3ies2 = double.Parse(_data); eles[31] = double.Parse(_data);
                            //
                        }
                        else if (c == 31)
                        {
                            m = double.Parse(_data); eles[32] = double.Parse(_data);
                        }
                        else if (c == 32)
                        {
                            br = double.Parse(_data); eles[33] = double.Parse(_data);
                        }
                        else if (c == 33)
                        {
                            ftime = double.Parse(_data); eles[34] = double.Parse(_data);
                        }
                        else if (c == 34)
                        {
                            fps = double.Parse(_data); eles[35] = double.Parse(_data);
                        }
                        else if (c == 35)
                        {
                            gtl2d = double.Parse(_data); eles[36] = double.Parse(_data);
                        }
                        else if (c == 36)
                        {
                            gtl3d = double.Parse(_data); eles[37] = double.Parse(_data);
                        }
                        else if (c == 37)
                        {
                            gtlcc = double.Parse(_data); eles[38] = double.Parse(_data);
                        }
                        else if (c == 38)
                        {
                            gtlta = double.Parse(_data); eles[39] = double.Parse(_data);
                        }
                        else if (c == 39)
                        {
                            gtt2d = double.Parse(_data); eles[40] = double.Parse(_data);
                        }
                        else if (c == 40)
                        {
                            gtt3d = double.Parse(_data); eles[41] = double.Parse(_data);
                        }
                        else if (c == 41)
                        {
                            gttcc = double.Parse(_data); eles[42] = double.Parse(_data);
                        }
                        else if (c == 42)
                        {
                            gttta = double.Parse(_data); eles[43] = double.Parse(_data);
                        }
                        else if (c == 43)
                        {
                            spm = double.Parse(_data); eles[44] = double.Parse(_data);
                        }
                        else if (c == 44)
                        {
                            isp = double.Parse(_data); eles[45] = double.Parse(_data);
                        }
                        else if (c == 45)
                        {
                            tal = double.Parse(_data); eles[46] = double.Parse(_data);
                        }
                        else if (c == 46)
                        {
                            usseccpp = double.Parse(_data); eles[47] = double.Parse(_data);
                        }
                        else if (c == 47)
                        {
                            usseccpv = double.Parse(_data); eles[48] = double.Parse(_data);
                        }
                        else if (c == 48)
                        {
                            usselp = double.Parse(_data); eles[49] = double.Parse(_data);
                        }
                        else if (c == 49)
                        {
                            usselv = double.Parse(_data); eles[50] = double.Parse(_data);
                        }
                        else if (c == 50)
                        {
                            vpf = double.Parse(_data); eles[51] = double.Parse(_data);
                        }
                        else if (c == 51)
                        {
                            vps = double.Parse(_data); eles[52] = double.Parse(_data);
                        }
                        //else if (c == 42)        
                        //power = double.Parse(_data);
                 
                    }

                    double avgU = (u0 + u1 + u2 + u3) / 4;
                    /*double avgIts0 = c0its0 + c1its0 + c2its0 + c3its0;
                    double avgIts1 = c0its1 + c1its1 + c2its1 + c3its1;
                    double avgIts2 = c0its2 + c1its2 + c2its2 + c3its2;
                    double avgIes0 = c0ies0 + c1ies0 + c2ies0 + c3ies0;
                    double avgIes1 = c0ies1 + c1ies1 + c2ies1 + c3ies1;
                    double avgIes2 = c0ies2 + c1ies2 + c2ies2 + c3ies2;


                    double c0 = Config.CPU400MHzS4Power(avgU, avgIts0, avgIts1, avgIts2, avgIes0, avgIes1, avgIes2, f0); */
                    /*double c0 = Config.CPU400MHzS4Power(u0, c1its0, c1its1, c1its2, c1ies0, c1ies1, c1ies2, f0);
                    double c1 = Config.CPU400MHzS4Power(u1, c1its0, c1its1, c1its2, c1ies0, c1ies1, c1ies2, f0);
                    double c2 = Config.CPU400MHzS4Power(u2, c2its0, c2its1, c2its2, c2ies0, c2ies1, c2ies2, f0);
                    double c3 = Config.CPU400MHzS4Power(u3, c3its0, c3its1, c3its2, c3ies0, c3ies1, c3ies2, f0);
                    double estModel = c0 +c1 + c2 + c3;
                    Console.WriteLine(estModel);
                    */

                    double estModel = 0;

                    if (f0 == 1600000)
                    {
                        estModel = 28.83 * avgU + 1028;
                    }
                    else if (f0 == 1200000)
                    {
                        estModel = 16.88 * avgU + 903.5;
                    }
                    else if (f0 == 900000)
                    {
                        estModel = 7.91 * avgU + 810;
                    }
                    else if (f0 == 600000)
                    {
                        estModel = 2.899 * avgU + 750;
                    }

                    
                    double measurePower = powers[r];

                    double estGPUpower = measurePower - estModel - 325; // 325 is the estimated of screen without RGB

                    if (estGPUpower < 0) estGPUpower = 0;

                    
                    for (int x = 0; x < eles.Length; x++)
                    {
                        if(x>=34)
                            values2 += eles[x] + " ";

                        values += eles[x] + " ";
                    }

                    values += measurePower;
                    values2 += estGPUpower;
                    
                    saveData.Add(values);
                    saveGPU.Add(values2);

                    values = "";
                    values2 = "";
                
                }

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                File.WriteAllLines(this.savePath + @"\raw_data_" + i + ".txt", toSave);
                saveData.Clear();


                string[] toSave2 = (string[])saveGPU.ToArray(typeof(string));
                File.WriteAllLines(this.savePath + @"\gpu_power_" + i + ".txt", toSave2);
                saveGPU.Clear();

            }
        }
    }
}
