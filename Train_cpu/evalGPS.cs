using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Train_DUT
{
    public class evalGPS
    {
        string samplePath = @"D:\research\Data\test_2";

        public evalGPS()
        {
            
        }

        public void execute2()
        {

            string[] files = Directory.GetFiles(samplePath);

            double[] powers = Tool.powerParseArr(files[8], 0, 5000);
           
            for (int j = 0; j < powers.Length; j++)
            {
                if (powers[j] <= 600 || powers[j] >= 950)
                {
                    powers[j] = 0;
                   
                }
            }

            double[] power4 = new double[85];

            for (int x = 530; x < (530+85); x++)
            {
                power4[x - 530] = powers[x]; 
            }

            string[] dataLines = File.ReadAllLines(samplePath + @"\gps_4.txt");


            int count = 0;
            string output = "";
            List<string> dat = new List<string>();

            for (int y = 1; y < dataLines.Length; y++)
            {
                string[] line = dataLines[y].Split('\t');

                if (line[4] == "20")
                {
                    for (int z = 0; z < line.Length; z++)
                    {
                        output += line[z] + " ";
                    }

                    dat.Add(output);
                }

                output = "";
            }

        }

        public void execute()
        {

            string[] files = Directory.GetFiles(samplePath);

            double[] powers = Tool.powerParseArr(files[8], 0, 5000);

            for (int j = 0; j < powers.Length; j++)
            {
                if (powers[j] <= 600 || powers[j] >= 950)
                {
                    powers[j] = 0;
                }
            }

            string[] powerStrs = new string[powers.Length];

            for(int i=0; i<powerStrs.Length; i++)
            {
                powerStrs[i] = powers[i].ToString();
            }

            string[] dataLines = File.ReadAllLines(files[3]);

            File.WriteAllLines(samplePath + @"\powerStr.txt", powerStrs);

            bool firstTime = true;
            
            ArrayList arr = new ArrayList();

            ArrayList arr2 = null;

            for (int k = 0; k < 1156; k++)
            {
                if (powers[k] == -1)
                {
                    if (arr2 != null)
                    {
                        arr.Add(arr2.Clone());
                        arr2.Clear();
                        arr2 = null;
                        firstTime = true;
                    }

                    continue;
                }

                if (firstTime)
                {
                    firstTime = false;
                    arr2 = new ArrayList();
                }

                arr2.Add(powers[k]);
                
            }

            ArrayList saveData = new ArrayList();

            for (int i = 1; i <= 7; i++)
            {
                string filePath = files[i - 1];
                string[] datas = File.ReadAllLines(filePath);

                for (int j = 0; j < datas.Length; j++)
                {
                    if (datas[j].Contains("bright=255")) continue;

                    if (j < ((ArrayList)arr[i - 1]).Count)
                    {
                        datas[j] += " power=" + ((ArrayList)arr[i - 1])[j].ToString();
                        saveData.Add(datas[j]);
                    }
                }

                string[] toSave = (string[])saveData.ToArray(typeof(string));
                File.WriteAllLines(this.samplePath + @"\output\gps_"+i+"_power.txt", toSave);
                saveData.Clear();
            }
        }
    }
}
