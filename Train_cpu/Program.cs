
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;
using System.IO;

namespace Train_DUT
{

    class Program
    {
        static void Main(string[] args)
        {

            int mode = 3; // 1=train, 2=evaluate, 3=screen, 4 = gps
            //Train
            if (mode == 1)
            {
                Train_CPU ts = new Train_CPU(2);
                ts.execute();
            }

            if (mode == 2)
            {
            }

            if (mode == 3)
            {
                new evalCPU().execute();
            }

            if (mode == 4)
            {
                new evalGPS().execute();
            }

            /*
            string[] files = Directory.GetFileSystemEntries(@"D:\SemiOnline\Experiment\Nexus\CPU_idle");

            for (int i = 0; i < files.Length; i++)
            {
                if (!files[i].Contains("test_"))
                {
                    Console.WriteLine(files[i]);
                    string[] splitNames = files[i].Split('\\');

                    string newString = "";

                    for (int j = 0; j < splitNames.Length-1; j++)
                    {
                        newString += splitNames[j] + @"\";
                    }

                    newString += "test_1_" + splitNames[splitNames.Length - 1];

                    File.Move(files[i], newString);
                }
            }
            */

            Console.WriteLine("Finish.");
            Console.ReadKey();
        }
    }
}
