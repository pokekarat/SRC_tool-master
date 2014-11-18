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

            Parse.ParseData();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            //textBox2.Text = Config.rootPath;
        }

      
    }
}
