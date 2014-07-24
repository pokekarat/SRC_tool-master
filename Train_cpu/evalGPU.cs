using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace Train_DUT
{
    public class evalGPU
    {
        string savePath = @"G:\SemiOnline\Experiment\Nexus\GPU";

        public evalGPU()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            Config.callProcess("chmod 777 /sys/class/backlight/s5p_bl/brightness");

            Thread.Sleep(5000);
        }

        public void Measure( )
        {
            int numTest = 10;

            for (int i = 8; i <= numTest; i++)
            {
                Config.callProcess("./data/local/tmp/OGLES2PVRScopeExample "+i+" &");

                Config.callPowerMeter(savePath + @"\power"+i+".pt4",180);

                Thread.Sleep(10000);

                //Config.pullFile("data/local/tmp/stat/sample" + i + ".txt", savePath);

                //Thread.Sleep(10000);
            }

            Thread.Sleep(30000);

            Config.pullFile("data/local/tmp/stat/", savePath);

            Thread.Sleep(5000);
        }

        public void Evaluate( )
        {

        }
         
    }
}
