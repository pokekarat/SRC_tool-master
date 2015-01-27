using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TrainDUTs
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        SolidBrush myBrush;
        Graphics formGraphics;

        private void Form1_Load(object sender, EventArgs e)
        {
            this.cb1.SelectedIndex = 0;
            Tool.init(statusTxt);

            //Nexus S
            Config.rootPath = @"G:\Semionline\tool\testSRCtool\cpu\";
            Config.adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //0=Nexus S
            int dut = cb1.SelectedIndex;
            if (dut == 0)
            {
               
                Config.brightPath = "/sys/class/backlight/s5p_bl/brightness";
                Config.freqs = new int[] { 200000, 400000, 800000, 1000000 };
                Config.cpuNums = new int[] { 0 };
                //Tool.ParseData();

            }
            //1=Galaxy S4
            else if (dut == 1)
            {

                //echo 1 > /sys/class/graphics/fb0/blank //make screen off.
                Config.blankPath = "/sys/class/graphics/fb0/blank";
                Config.brightPath = "/sys/class/backlight/panel/brightness";
                Config.freqs = new int[] { 800000 }; //250000, 300000, 400000, 500000, 600000, 700000, 800000, 1000000, 1200000, 1400000, 1600000 };
                Config.cpuNums = new int[] { 0 };


                for (int i = 0; i < 5; i++)
                {

                    Config.callProcess("echo 1 > " + Config.blankPath);

                    Thread.Sleep(2000);

                    Config.callProcess("echo 0 > " + Config.blankPath);

                    Thread.Sleep(2000);
                }

            }

            //trainCPu.train();
            myBrush = new SolidBrush(System.Drawing.Color.Green);
            formGraphics = this.CreateGraphics();
            formGraphics.FillRectangle(myBrush, new Rectangle(60, 80, 20, 15));
            myBrush.Dispose();
            formGraphics.Dispose();

        }

        private void sample_btn_Click(object sender, EventArgs e)
        {
            Config.fileIndex = int.Parse(this.fileIndex_tb.Text);
            Config.duration = int.Parse(this.sampleTime_tb.Text);

            Tool.sampleData();
        }

        private void parseBtn_Click(object sender, EventArgs e)
        {
            Tool.ParseData();
        }
    }
}
