using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
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
         

        }

        private void button1_Click(object sender, EventArgs e)
        {
            //0=Nexus S
            if (cb1.SelectedIndex == 0)
            {
                //Nexus S
                Config.rootPath = @"C:\Users\pok\Semionline\";
                Config.adbPath = @"C:\Users\pok\android\sdk\platform-tools\";
                Config.brightPath = "/sys/class/backlight/s5p_bl/brightness";
                Config.freqs = new int[] { 100000, 200000, 400000, 800000, 1000000 };
                Config.cpuNums = new int[] { 0 };

                //trainCPu.train();
                myBrush = new SolidBrush(System.Drawing.Color.Green);
                formGraphics = this.CreateGraphics();
                formGraphics.FillRectangle(myBrush, new Rectangle(60, 80, 20, 15));
                myBrush.Dispose();
                formGraphics.Dispose();
              
            }
        }
    }
}
