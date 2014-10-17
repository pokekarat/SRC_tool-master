﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Train_DUT
{
    public class testApp
    {
        public static void parseData()
        {

            int fileIndex = 1;

            Config.callProcess2("pull data/local/tmp/stat/sample"+fileIndex+@".txt " + Config.rootPath+"sample"+fileIndex+@".txt");

            string savePath = Config.rootPath;
            //string header = "util freq idle_time idle_usage bright tx rx up ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf power";
            string header = "util0 freq0 it ie bright tx rx cap volt temp ftime fps g3d_core gta_core g3d_time gta_time ta_load txt_uld usse_cc_pix usse_cc_ver usse_load_pix usse_load_ver vpf";
            

            List<List<string>> lists = new List<List<string>>();

            
            ArrayList saveData = new ArrayList();

            for (int i = fileIndex; i <= fileIndex; i++)
            {

                string inputFileName = savePath + "sample" + i + ".txt";
                if (!File.Exists(inputFileName)) continue;
                
                string[] datas = File.ReadAllLines(inputFileName);
                //double[] powers = Tool.powerParseArr(i, savePath, 0, 5000);

                lists = Config.processData(datas);

                int row = lists.Count-1;
                int col = 0; // lists[0].Count;
                string values = "";       
                saveData.Add(header);
               
                for (int r = 1; r < row; r++)
                {
                    
                    List<string> curData = lists[r];

                    col = curData.Count;

                    for (int c = 1; c < col; c++)
                    {         
                        
                        values += curData[c] + " ";
                    }

                   // values += powers[r];

                    saveData.Add(values);

                    values = "";
                }

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                string saveName = Config.rootPath + "raw_data_" + i + ".txt";
                Console.WriteLine("File save = " + saveName);
                File.WriteAllLines(saveName, toSave);
                saveData.Clear();

            }
        }
    }
}