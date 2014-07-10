
using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Text.RegularExpressions;

namespace Train_DUT
{

    class Program
    {
        static void Main(string[] args)
        {

            int mode = 3; // 1=train, 2=evaluate, 3=screen
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
                new Train_SCREEN().execute();
            }

            Console.WriteLine("Finish.");
            Console.ReadKey();
        }
    }
}
