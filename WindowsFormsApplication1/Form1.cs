
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using WIA;


namespace WindowsFormsApplication1
{

    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();
            this.button1.Text = "Select Camera";
            this.label1.Text = "[ no camera selected ]";
        }

        private void Form1_Load(object sender, EventArgs e) { }
        private String _label = null;
        private String _id = "";
        DeviceManager mng = new DeviceManager();
        private void button1_Click_1(object sender, EventArgs e)
        {
            try
            {
                // create a new WIA common dialog box for the user to select a device from
                WIA.CommonDialog dlg = new WIA.CommonDialog();

                // show user the WIA device dialog
                Device d = dlg.ShowSelectDevice(WiaDeviceType.CameraDeviceType, true, false);
                _id = d.DeviceID;
                // check if a device was selected
                if (d != null)
                {

                    // Print camera properties
                    /*richTextBox1.AppendText("\n\n Print properties:\n");
                    foreach (Property p in d.Properties)
                    {
                        richTextBox1.AppendText(p.Name + ": " + p.get_Value() + "  (" + p.PropertyID + ":" + p.IsReadOnly + ") \n");

                        // Update UI
                        if (p.PropertyID == 3) _label = (String)p.get_Value();
                        if (p.PropertyID == 4) _label = _label + " - " + p.get_Value();
                        this.label1.Text = _label;
                    }

                    // Print commands
                    richTextBox1.AppendText("\n\n Print commands:\n");
                    foreach (DeviceCommand dvc in d.Commands)
                    {
                        richTextBox1.AppendText(dvc.Name + ": " + dvc.Description + "  (" + dvc.CommandID + ") \n");
                    }

                    // Print events
                    richTextBox1.AppendText("\n\n Print events:\n");
                    foreach (DeviceEvent dve in d.Events)
                    {
                        richTextBox1.AppendText(dve.Name + ": " + dve.Description + "  (" + dve.Type + ") \n");
                    }

                    // Print item properties
                    richTextBox1.AppendText("\n\n Print item properties:\n");
                    /*foreach (Property item in d.Items[1].Properties)
                    {
                        richTextBox1.AppendText(item.IsReadOnly + ": " + item.Name + "  (" + item.PropertyID + ") \n");
                    }*/


                    /*foreach (WIA.Property p in d.Properties)
                    {
                        Object tempNewProperty;


                        // change Exposure Compensation: value 0 to 2 (ID 2053, isReadonly False)
                        if (p.PropertyID == 2053)
                        {
                            tempNewProperty = (int)-2000;  // can not be set to minus values, why???
                            ((IProperty)p).set_Value(ref tempNewProperty);
                            richTextBox1.AppendText(">>>>" + p.get_Value());
                        }
                    }*/

                    // Now let's take a picture !
                    //

                    foreach (DeviceInfo info in mng.DeviceInfos)
                    {
                        if (info.DeviceID == _id)
                        {
                            d = info.Connect();
                            break;
                        }
                    }

                    d.ExecuteCommand(WIA.CommandID.wiaCommandTakePicture);

                    mng.RegisterEvent(EventID.wiaEventItemCreated, Miscellaneous.wiaAnyDeviceID);
                    Item item = d.ExecuteCommand(CommandID.wiaCommandTakePicture);
                    //richTextBox1.AppendText(".");
                    //Item item = d.ExecuteCommand(CommandID.wiaCommandTakePicture);

                }
                else
                {
                    d = null;
                    richTextBox1.AppendText("Result: no device selected or device could not be read. ");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "WIA Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            //DeviceManager mng = new DeviceManager();
            Device d = null;

            foreach (DeviceInfo info in mng.DeviceInfos)
            {
                if (info.DeviceID == _id)
                {
                    d = info.Connect();
                    break;
                }
            }

            //Item item = d.ExecuteCommand(CommandID.wiaCommandTakePicture);
            //foreach (string format in item.Formats)
            {
                /*if (format == jpegGuid)
                {
                    WIA.ImageFile imagefile = item.Transfer(format) as WIA.ImageFile;
                    filename = GetFreeFileName();
                    if (string.IsNullOrEmpty(filename) == false)
                    {
                        imagefile.SaveFile(filename);
                    }
                    this.picLastImage.Load(filename);
                    return filename;
                }*/
            }
        }
    


    }


}


