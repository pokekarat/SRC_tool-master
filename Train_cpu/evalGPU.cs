using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Train_DUT
{
    public class evalGPU
    {
        string savePath = @"D:\SemiOnline\Experiment\Nexus\GPU\2D";

        public evalGPU()
        {
            
        }

        public void Measure( )
        {
            int numTest = 7;

            for (int i = 3; i <= numTest; i++)
            {
                Config.callProcess("./data/local/tmp/OGLES2PVRScopeExample "+i+" &");

                Config.callPowerMeter(savePath + @"\power"+i+".pt4",190);

                Thread.Sleep(10000);

                //Config.pullFile("data/local/tmp/stat/sample" + i + ".txt", savePath);

                //Thread.Sleep(10000);
            }

            Config.pullFile("data/local/tmp/stat/", savePath);

            Thread.Sleep(10000);
        }

        public void Evaluate( )
        {

        }
         
    }
}
