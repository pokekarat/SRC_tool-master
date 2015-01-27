namespace TrainDUTs
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.cb1 = new System.Windows.Forms.ComboBox();
            this.button1 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.statusTxt = new System.Windows.Forms.TextBox();
            this.sample_btn = new System.Windows.Forms.Button();
            this.fileIndex_tb = new System.Windows.Forms.TextBox();
            this.sampleTime_tb = new System.Windows.Forms.TextBox();
            this.parseBtn = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // cb1
            // 
            this.cb1.FormattingEnabled = true;
            this.cb1.Items.AddRange(new object[] {
            "Nexus S",
            "Galaxy S4"});
            this.cb1.Location = new System.Drawing.Point(60, 20);
            this.cb1.Name = "cb1";
            this.cb1.Size = new System.Drawing.Size(121, 21);
            this.cb1.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(251, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "Train";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(86, 83);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(29, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "CPU";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(86, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(28, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "LCD";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(86, 141);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(30, 13);
            this.label3.TabIndex = 2;
            this.label3.Text = "GPU";
            // 
            // statusTxt
            // 
            this.statusTxt.Location = new System.Drawing.Point(60, 217);
            this.statusTxt.Multiline = true;
            this.statusTxt.Name = "statusTxt";
            this.statusTxt.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.statusTxt.Size = new System.Drawing.Size(266, 112);
            this.statusTxt.TabIndex = 3;
            // 
            // sample_btn
            // 
            this.sample_btn.Location = new System.Drawing.Point(251, 131);
            this.sample_btn.Name = "sample_btn";
            this.sample_btn.Size = new System.Drawing.Size(75, 23);
            this.sample_btn.TabIndex = 4;
            this.sample_btn.Text = "Sample";
            this.sample_btn.UseVisualStyleBackColor = true;
            this.sample_btn.Click += new System.EventHandler(this.sample_btn_Click);
            // 
            // fileIndex_tb
            // 
            this.fileIndex_tb.Location = new System.Drawing.Point(251, 161);
            this.fileIndex_tb.Name = "fileIndex_tb";
            this.fileIndex_tb.Size = new System.Drawing.Size(75, 20);
            this.fileIndex_tb.TabIndex = 5;
            this.fileIndex_tb.Text = "1";
            // 
            // sampleTime_tb
            // 
            this.sampleTime_tb.Location = new System.Drawing.Point(251, 191);
            this.sampleTime_tb.Name = "sampleTime_tb";
            this.sampleTime_tb.Size = new System.Drawing.Size(75, 20);
            this.sampleTime_tb.TabIndex = 6;
            this.sampleTime_tb.Text = "60";
            // 
            // parseBtn
            // 
            this.parseBtn.Location = new System.Drawing.Point(251, 60);
            this.parseBtn.Name = "parseBtn";
            this.parseBtn.Size = new System.Drawing.Size(75, 23);
            this.parseBtn.TabIndex = 7;
            this.parseBtn.Text = "Parse";
            this.parseBtn.UseVisualStyleBackColor = true;
            this.parseBtn.Click += new System.EventHandler(this.parseBtn_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(393, 360);
            this.Controls.Add(this.parseBtn);
            this.Controls.Add(this.sampleTime_tb);
            this.Controls.Add(this.fileIndex_tb);
            this.Controls.Add(this.sample_btn);
            this.Controls.Add(this.statusTxt);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.cb1);
            this.Name = "Form1";
            this.Text = "TrainDUTs";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cb1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox statusTxt;
        private System.Windows.Forms.Button sample_btn;
        private System.Windows.Forms.TextBox fileIndex_tb;
        private System.Windows.Forms.TextBox sampleTime_tb;
        private System.Windows.Forms.Button parseBtn;




    }
}

