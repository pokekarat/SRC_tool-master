using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Train_DUT
{
    public class evalWiFi
    {
        public evalWiFi()
        {
            
        }

        public void execute2(string channel)
        {
         
            
            string[] sysFiles = Directory.GetFiles(Config.rootPath);
            string[] powFiles = Directory.GetFiles(Config.rootPath + @"\power\output");

            for (int i = 0; i < sysFiles.Length; i++)
            {
                string[] sysLines = File.ReadAllLines(sysFiles[i]);
                string[] powLines = File.ReadAllLines(powFiles[i]);

                int sysLen = sysLines.Length;
                string[] toSave = new string[sysLen];
                List<string> toSave2 = new List<string>();
                toSave2.Add("channel numpack power");

                for (int j = 0; j < sysLen; j++)
                {
                    string sysLine = sysLines[j];
                    string measurePowStr = powLines[j];

                    //Console.WriteLine(sysLine + " power=" + measurePowStr);

                    int np = 0;
                    double cpu = 0;

                    for (int k = 0; k < sysLine.Split(' ').Length; k++)
                    {
                        string[] element = sysLine.Split(' ')[k].Split('=');

                        if (element[0].Equals("cpu"))
                        {
                            Double.TryParse(element[1], out cpu);
                        }
                        else if (element[0].Equals("np"))
                        {
                            Int32.TryParse(element[1], out np);
                        }
                    }

                    double estCpuPower = Config.NEXUS_CPU_POWER_MODEL(1000, cpu);
                    double measurePow = 0;
                    Double.TryParse(measurePowStr, out measurePow);

                    double subPower = measurePow - estCpuPower;

                    if (subPower < 0) subPower = -1;
                    if (np < 3) np = -1;

                    string toSaveLine = np + " " + subPower;
                    toSave[j] = toSaveLine;

                    //Console.WriteLine(toSaveLine);
                
                }

                Dictionary<int, ArrayList> collection = new Dictionary<int, ArrayList>();

                int order = 0;
                
                for (int m = 0; m < toSave.Length; m++)
                {
                    string[] dataS = toSave[m].Split(' ');

                    int head = int.Parse(dataS[0]);
                    double end = Double.Parse(dataS[1]);

                    if (end != -1 )
                    {

                        if (i == 4 && m == 221)
                        {
                            Console.WriteLine("debug");
                        }

                        ArrayList arr = new ArrayList();
                        arr.Add(toSave[m]);

                        int forward = m+1;
                        int backward = m-1;

                        if (m != 0 && m != toSave.Length - 1)
                        {
                            
                            //Move forward
                            while (!toSave[forward].Equals("-1 -1"))
                            {
                                arr.Add(toSave[forward]);
                                ++forward;

                                if (forward == toSave.Length) break;
                            }

                            //Move backward               
                            while (!toSave[backward].Equals("-1 -1"))
                            {
                                arr.Add(toSave[backward]);
                                --backward;

                                if (backward < 0) break;
                            }
                        }
                        else
                        {
                            if (m == 0)
                            {
                                //Move forward
                                while (!toSave[forward].Equals("-1 -1"))
                                {
                                    arr.Add(toSave[forward]);
                                    ++forward;

                                    if (forward == toSave.Length) break;
                                }
                            }
                            else if (m == (toSave.Length - 1))
                            {
                                //Move backward               
                                while (!toSave[backward].Equals("-1 -1"))
                                {
                                    arr.Add(toSave[backward]);
                                    --backward;

                                    if (backward < 0) break;
                                }
                            }
                        }

                        m = forward;

                        collection[order] = arr;
                        ++order;
                    }   
                }

                int[] key = collection.Keys.ToArray();

                List<int> npData = new List<int>();

                List<double> powData = new List<double>();

                for (int k = 0; k < key.Length; k++)
                {
                    ArrayList ar = collection[k];

                    for (int j = 0; j < ar.Count; j++ )
                    {
                        string getData = (string)ar[j];

                        string[] _getData = getData.Split(' ');
                        npData.Add(int.Parse(_getData[0]));
                        powData.Add(double.Parse(_getData[1]));

                    }

                    npData.Sort();
                    powData.Sort();


                    if (npData[npData.Count - 1] != -1)
                    {
                        toSave2.Add(channel + " " + npData[npData.Count - 1] + " " + powData[powData.Count - 1]);
                    }

                    npData.Clear();
                    powData.Clear();
                }

                if (!Directory.Exists(Config.rootPath + @"\output"))
                {
                    Directory.CreateDirectory(Config.rootPath + @"\output");
                }

                File.WriteAllLines(Config.rootPath + @"\output\output_" + (i + 1) + ".txt", toSave2);
                File.WriteAllLines(Config.rootPath + @"\output\test_output_" + (i + 1) + ".txt", toSave);

                toSave2.Clear();
            }
        }

        public void execute()
        {
            string rootPath = @"D:\SemiOnline\Experiment\Nexus\WiFi\channel_11";

            Dictionary<int, double> collectData = new Dictionary<int, double>();

            string testStr = "";

            ArrayList results = new ArrayList();

            int saveIndex = 0;
            
            string[] files = Directory.GetFiles(rootPath);
            
            double[] powers = Tool.powerParseArr(rootPath + @"\power", 0, files.Length, 5000);

            string[] toSavePower = new string[powers.Length];
            for(int i=0; i<powers.Length; i++)
                toSavePower[i] = Math.Round(powers[i],2).ToString();

            File.WriteAllLines(rootPath + @"\power.txt", toSavePower);

            //List all files
            for (int i = 0; i < files.Length; i++)
            {

                if (files[i].Contains("wifi"))
                {
                    //results.Add("np power");
                    ++saveIndex;
                    //int samplePower = 0;
                    string[] data = File.ReadAllLines(files[i]);

                    //List all data in file
                    //Consider only np > 2
                    for (int j = 0; j < data.Length; j++)
                    {

                        string[] elements = data[j].Split(' ');
                    
                        int sample = 0;
                        int.TryParse(elements[0].Split('=')[1], out sample);

                        int npTest = -1;
                        int.TryParse(elements[9].Split('=')[1], out npTest);

                        double fTest = -1;
                        Double.TryParse(elements[3].Split('=')[1], out fTest);

                        //Console.WriteLine("fTest " + fTest+ " npTest "+npTest);
                        //collectData[sample] = "*";

                        if (npTest <= 2 ) //|| (fTest != 200.0 && fTest != 400.0 && fTest != 800.0 && fTest != 1000.0))
                        {
                            testStr += "*";
                            continue;
                        }
                        else
                        {
                            //collectData[sample] = data[j];
                            
                          
                           /* if (saveIndex > 1)
                            {

                                if (j == 0)
                                {
                                    samplePower = sample;
                                    double power = 0;
                                    do
                                    {
                                        power = Math.Round(powers[samplePower], 2);
                                        ++samplePower;
                                    } while (power > 450);
                                }
                                else
                                {
                                    samplePower += (j);
                                }
                            }
                            else
                            {
                                samplePower = sample - 1;
                            } */

                            double power = Math.Round(powers[sample], 2);
                            testStr += data[j] + " power="+ power  + "%";
                        }
                    }
                    
                   string[] split = Regex.Split(testStr, "\\*");

                   ArrayList arr = new ArrayList();
                   
                   for (int a = 0; a < split.Length; a++)
                   {
                       if (split[a] == "") continue;
                       arr.Add(split[a]);
                   }

                   for (int a = 0; a < arr.Count; a++)
                   {
                       string[] dataLine = ((string)arr[a]).Split('%');

                       List<double> accCPU = new List<double>();
                       List<double> accPW = new List<double>();
                       List<double> accNP = new List<double>();

                       double sumCpu = 0.0, sumFreq = 0.0, sumPower = 0.0;
                       int sumNp = 0;

                       double cpu = 0.0, freq = 0.0, power = 0.0;
                       int np = 0;

                       int size = dataLine.Length-1;

                       for (int b = 0; b < size; b++)
                       {
                           if (dataLine[b] != "")
                           {
                               string _dataLine = dataLine[b];
                               string[] arrDataLine = _dataLine.Split(' ');
                               
                               for (int c = 0; c < arrDataLine.Length; c++)
                               {
                                   string paramName = arrDataLine[c].Split('=')[0];
                                   string paramData = arrDataLine[c].Split('=')[1];

                                   if (paramName == "cpu")
                                   {
                                       Double.TryParse(paramData, out cpu);
                                       sumCpu += cpu;
                                       accCPU.Add(cpu);
                                   }
                                   else if (paramName == "f")
                                   {
                                       Double.TryParse(paramData, out freq);
                                       sumFreq += freq;
                                   }
                                   else if (paramName == "np")
                                   {
                                       Int32.TryParse(paramData, out np);
                                       sumNp += np;
                                       accNP.Add(np);
                                   }
                                   else if (paramName == "power")
                                   {
                                       Double.TryParse(paramData, out power);
                                       sumPower += power;
                                       accPW.Add(power);
                                   }
                               }  
                           }
                       }

                       accNP.Sort();
                       accPW.Sort();
                       
                       double avgCpu = accCPU.Average();
                       //double avgFreq = 0;
                       int avgNp = (int)accNP[accNP.Count - 1];
                       double avgPower = (double)accPW[accPW.Count - 1];

                       accCPU.Clear();
                       accNP.Clear();
                       accPW.Clear();

                       //1000 MHZ
                       double cpuPowerEst = (-1.131 * 0.26) + (6.047 * avgCpu) + 468.957;

                       double subtractPw = avgPower - cpuPowerEst;

                       if (subtractPw < 0) continue;

                       Console.WriteLine(avgNp + " " + subtractPw);

                       results.Add(avgNp + " " + Math.Round(subtractPw, 2));

                       collectData[avgNp] = Math.Round(subtractPw, 2);

                   }

                   string[] toSave = new string[collectData.Count+1]; 
                   toSave[0] = "np power";
                   int t = 1;
                   foreach (KeyValuePair<int, double> npData in collectData.OrderBy(key => key.Key))
                   {
                       Console.WriteLine("Key: {0}, Value: {1}", npData.Key, npData.Value);
                       toSave[t] = npData.Key + " " + npData.Value;
                       ++t;
                   }

                  
                    //Save process sample data
                   // string[] toSave = (string[])results.ToArray(typeof(string));
                    
                    File.WriteAllLines(rootPath + @"\output\wifi_power_" + saveIndex + ".txt", toSave);

                    results.Clear();
                    collectData.Clear();
                    testStr = "";
                }
            }
        }
    }
}
