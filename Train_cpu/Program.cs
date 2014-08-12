
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

            int mode = 4; // 1=train, 2=evaluate, 3=screen, 4 = gps

            if (mode == 11)
            {
                evalGPU.extractVideoFrame();
            }

            if (mode == 10)
            {
                Config.parseDisplayData();
            }


            if (mode == 9)
            {
                //Config.measure();
                Config.trainS4cpu();
            }

            else if (mode == 8)
            {
                testApp.Evaluate();
            }

            //Train
            else if (mode == 1)
            {
                Train_CPU ts = new Train_CPU(2);
                ts.execute();
            }

            else if (mode == 2)
            {

            }

            else if (mode == 3)
            {
                new evalCPU().execute();
            }

            else if (mode == 4)
            {
                new evalGPS().execute2();
            }

            else if (mode == 5)
            {
                new evalWiFi().execute2("36");
            }

            else if (mode == 6)
            {
                //Tool.powerPartition(Config.rootPath+@"\power", 30, 264);
                string[] channel = Config.rootPath.Split('_');
                new evalWiFi().execute2(channel[1]);
            }

            else if (mode == 7)
            {
               evalGPU eg = new evalGPU();
               
               eg.Evaluate2();
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
            System.Media.SystemSounds.Asterisk.Play();
            Console.ReadKey();
        }
    }
}
