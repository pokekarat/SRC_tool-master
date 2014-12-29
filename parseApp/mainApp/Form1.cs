using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mainApp
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Config.rootPath = textBox2.Text;
            //Config.adbPath = textBox1.Text;
            Config.sampleFileIndex = int.Parse(textBox3.Text);
            Config.sampleFileIndexEnd = int.Parse(textBox4.Text);

            Parse.ParseData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //textBox2.Text = Config.rootPath;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //Build sod curve
            // 1. load sod curve data.
            string[] datas = File.ReadAllLines(@"C:\Users\pok\Research\Experiment\Dropbox\Project2_SemiOnline\Experiment\S4\SOD\sod.csv");
            Dictionary<int, int> sodCurve = new Dictionary<int, int>();

            for (int i = 1; i < datas.Length; i++)
            {      
                string[] pairs = datas[i].Split(',');
                int k = int.Parse(pairs[0]);
                int v = int.Parse(pairs[1]);

                //if dict is empty
                if (sodCurve.Count == 0)
                {
                    sodCurve[k] = v;
                }
                else
                {
                    if (!sodCurve.ContainsValue(v))
                    {
                        sodCurve[k] = v;
                    }
                }
            }

            //finish build sodcurve
            
            //Test
            string testFile1 = Config.rootPath + "raw_data_2.txt";
            string[] fileDatas = File.ReadAllLines(testFile1);
            
            string[] lines = fileDatas[2].Split(' ');
            int volt_begin = int.Parse(lines[45]);

            lines = fileDatas[fileDatas.Length-1].Split(' ');
            int volt_end = int.Parse(lines[45]);

            int cap_begin = 0;
            int cap_end = 0;

            //Map volt_* with sodCurve
            //int previousKey;
            //int previousValue;

            foreach (var key in sodCurve.Keys)
            {
                var value = sodCurve[key];

                if (volt_begin > key)
                {
                    cap_begin = value;
                    break;
                }

                //previousKey = key;
                //previousValue = value;
            }

            foreach (var key in sodCurve.Keys)
            {
                var value = sodCurve[key];

                if (volt_end > key)
                {
                    cap_end = value;
                    break;
                }
            }

            int batt_cap = 2600;
            int time_use = 30; //min
            int diff_cap = cap_begin - cap_end;
            int est_current = (diff_cap * batt_cap * 60) / (time_use * 100); 

        }
    }
}
