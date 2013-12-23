using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GameProfileSaver
{
    public partial class SettingsForm : Form
    {
        private Form1 form1;

        public SettingsForm(Form1 frm)
        {
            InitializeComponent();
            form1 = frm;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("GameProfileSaver") == null)
            {
                checkBox2.Checked = false;
            }

            else
            {
                checkBox2.Checked = true;
            }
        }

        private void SettingsForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                form1.Show();
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (checkBox2.Checked == true)
            {
                rkApp.SetValue("GameProfileSaver", Application.ExecutablePath.ToString());
            }
            else
            {
                rkApp.DeleteValue("GameProfileSaver", false);
            }

            Close();
        }

    }
}
