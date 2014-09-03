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
        public static void parseS4()
        {
            int numApp = 7;

            for (int t = numApp; t <= numApp; t++)
            {
                string savePath = @"D:\research\S4\Real_Test\app"+t;
                //string pixelFile = savePath + @"\pixelPower.txt";

                List<List<string>> lists = new List<List<string>>();

                int numFiles = 1;
               
                ArrayList saveData = new ArrayList();

                for (int i = 1; i <= numFiles; i++)
                {

                    string inputFileName = savePath + @"\sample" + i + ".txt";
                    if (!File.Exists(inputFileName)) continue;

                    string[] datas = File.ReadAllLines(inputFileName);
                    //string[] pixels = File.ReadAllLines(pixelFile);
                    double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                    lists = Config.processDataS4(datas);

                    int row = lists.Count - 1;
                    int col = 0; // lists[0].Count;
                    string values = "";
                    //int pc = Config.paramName.Count;

                    saveData.Add("util0 util1 util2 util3 freq0 freq1 freq2 freq3 " +
                        "c0its0 c0its1 c0its2 c1its0 c1its1 c1its2 c2its0 c2its1 c2its2 c3its0 c3its1 c3its2 " +
                        "c0ies0 c0ies1 c0ies2 c1ies0 c1ies1 c1ies2 c2ies0 c2ies1 c2ies2 c3ies0 c3ies1 c3ies2 " +
                        "mem bright tx rx up " +
                        /*"ftime fps g2d_core g3d_core glc_core gta_core g2d_core g3d_core gtc_core gta_time " +
                        "spm isp ta_load usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf vps power");*/

                    "ftime fps gtl2d gtl3d gtlcc gtlta gtt2d gtt3d gttcc gttta spm isp tal usseccpp usseccpv usselp usselv vpf vps powers");

                    //10 is sync with power
                    for (int r =0; r < row; r++)
                    {
                        List<string> curData = lists[r];

                        col = curData.Count;

                        if (curData[36] == "up") curData[36] = "1.000";
                        if (curData[36] == "down") curData[36] = "0.000";

                        for (int c = 0; c < col; c++)
                        {

                            values += curData[c] + " ";
                        }

                        if (r >= 10)
                            values += powers[r - 9];
                        else
                            values += 0;

                        saveData.Add(values);

                        values = "";
                    }

                    string[] toSave = (string[])saveData.ToArray(typeof(string));
                    string saveName = savePath + @"\raw_data_" + i + ".txt";
                    Console.WriteLine("File save = " + saveName);
                    File.WriteAllLines(saveName, toSave);
                    saveData.Clear();


                }
            }
        }

        public static void parseNexusS()
        {
            string savePath = @"D:\research\Nexus\Real_Test\app7";

            List<List<string>> lists = new List<List<string>>();

            int numFiles = 7;
            ArrayList saveData = new ArrayList();

            for (int i = 1; i <= numFiles; i++)
            {

                string inputFileName = savePath + @"\sample" + i + ".txt";
                if (!File.Exists(inputFileName)) continue;
                
                string[] datas = File.ReadAllLines(inputFileName);
                double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                lists = Config.processData(datas);

                int row = lists.Count-1;
                int col = 0; // lists[0].Count;
                string values = "";       
                saveData.Add("util freq idle_time idle_usage bright tx rx up ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power");
               
                for (int r = 0; r < row; r++)
                {
                    
                    List<string> curData = lists[r];
                    curData.RemoveAt(4);
                    col = curData.Count;

                    if (curData[4] == "0") continue;

                    if (curData[7] == "up") curData[7] = "1.000";
                    if (curData[7] == "down") curData[7] = "0.000";

                    for (int c = 0; c < col; c++)
                    {         
                        
                        values += curData[c] + " ";
                    }

                    values += powers[r];

                    saveData.Add(values);

                    values = "";
                }

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                string saveName = savePath + @"\raw_data_" + i + ".txt";
                Console.WriteLine("File save = " + saveName);
                File.WriteAllLines(saveName, toSave);
                saveData.Clear();



            }
        }
    }
}
