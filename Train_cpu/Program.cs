
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Train_cpu
{

    class Program
    {
        static void Main(string[] args)
        {

            int mode = 1; // 1=train, 2=evaluate
            //Train
            if (mode == 1)
            {
                Train_CPU ts = new Train_CPU(2);
                ts.execute();
            }
            else
            {

                //Evaluation
                double energySum = 0;
                for (int i = 1; i <= 5; i++)
                {
                    string path = @"C:\Users\pok\Semionline\CPU_idle_7_2_2014\" + i;
                    double power = Tool.powerParse(path);
                    double energy = (power * 100);
                    Console.WriteLine("Energy " + i + " = " + energy); //100 seconds
                    energySum += energy;

                }

                Console.WriteLine("Average = " + energySum / 5);
            }

            Console.WriteLine("Finish.");
            Console.ReadKey();
        }
    }
}
