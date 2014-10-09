using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Train_DUT
{
    public class evalScreen
    {
        static string savePath = @"F:\Semionline\Experiment\Nexus\Display\1";

        public evalScreen()
        {
            
        }

        public static void execute()
        {
            string[] datas = File.ReadAllLines(savePath + @"\LCD_250.txt");
            double[] powers = Tool.powerParseArr(1, savePath, 0, 5000);
            ArrayList saveData = new ArrayList();
            saveData.Add("util freq bright power");
            for (int i = 0; i < datas.Length; i++)
            {
                string output = datas[i].Trim();

                string[] line = output.Split(' ');

                int powerIndex = Int32.Parse(line[0]);
                double pw = powers[powerIndex];


                string line2 = line[1].Remove(0, 1);
                line2 = line2.Remove(line2.Length - 1, 1);
                string[] cpu = line2.Split(',');

                string line3 = line[2].Remove(0, 1);
                line3 = line3.Remove(line3.Length - 1, 1);
                string[] bright = line3.Split(',');


                saveData.Add(cpu[0] + " " + cpu[1] + " " + bright[0] + " " + pw);
            }

            string[] toSave = (string[])saveData.ToArray(typeof(string));
            string saveName = savePath + @"\raw_data_1.txt";
            Console.WriteLine("File save = " + saveName);
            File.WriteAllLines(saveName, toSave);
            saveData.Clear();

        }
    }
}
