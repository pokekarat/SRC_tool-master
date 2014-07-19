using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Train_DUT
{
    public class Config
    {
        public static string rootPath = @"D:\SemiOnline\Experiment\Nexus\WiFi\channel_54";

        public static double NEXUS_CPU_POWER_MODEL(double freq, double cpu)
        {
            
            double estPower = 0;

            if(freq == 1000.0)
                estPower = (-1.131 * 0.26) + (6.047 * cpu) + 468.957;

            return estPower;
        }
    }
}
