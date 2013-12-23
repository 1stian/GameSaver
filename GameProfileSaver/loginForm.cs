using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GameProfileSaver
{
    public partial class loginForm : Form
    {
        int dlCount = 0;
        int done = 0;

        public loginForm()
        {
            InitializeComponent();
        }

        public bool deCheckAuto = false;

        private void loginForm_Load(object sender, EventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            //label4.Text = "Checking files...";
            if (!File.Exists(gpsPath + @"\ICSharpCode.SharpZipLib.dll") || !File.Exists(gpsPath + @"\ICSharpCode.SharpZipLib.dll"))
            {
                //startDownload("dll");
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = true;
                label4.Text = "File check complete. Ready for login.";
                progressBar1.Value = 100;
            }

            if (Properties.Settings.Default.check1 == true)
            {
                checkBox1.Checked = true;
            }
            if (Properties.Settings.Default.check2 == true)
            {
                checkBox2.Checked = true;
            }

            usernameText.Text = Properties.Settings.Default.username;

            if (checkBox1.Checked == true)
            {
                passwordText.Text = Properties.Settings.Default.password;
            }

            if (checkBox2.Checked == true)
            {
                button1.PerformClick();
            }
        }

        private void startLoginWorker()
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            this.Hide();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(openRegisterForm));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void openRegisterForm()
        {
            Application.Run(new registerForm(this));
        }

        private string GetSHA1HashData(string data)
        {
            //create new instance of md5
            SHA1 sha1 = SHA1.Create();

            //convert the input text to array of bytes
            byte[] hashData = sha1.ComputeHash(Encoding.UTF8.GetBytes(data));

            //create new instance of StringBuilder to save hashed data
            StringBuilder returnValue = new StringBuilder();

            //loop for each byte and add it to StringBuilder
            for (int i = 0; i < hashData.Length; i++)
            {
                returnValue.Append(hashData[i].ToString());
            }

            // return hexadecimal string
            return returnValue.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = "Login in... Please wait.";
            Properties.Settings.Default.username = usernameText.Text;
            if (checkBox1.Checked == true)
            {
                Properties.Settings.Default.check1 = true;
                Properties.Settings.Default.password = passwordText.Text;
                if (checkBox2.Checked == true)
                {
                    Properties.Settings.Default.check2 = true;
                }
            }
            Properties.Settings.Default.Save();
            button1.Enabled = false;
            checkBox1.Enabled = false;
            checkBox2.Enabled = false;
            startLoginWorker();
        }

        private void oppenMainForm()
        {
            Application.Run(new Form1(usernameText.Text));
        }

        private void startDownload(string type)
        {
            try
            {
                String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
                String gpsPath = appDataFolder + "/GameProfileSaver";

                string dName = null;
                string dId = null;
                string dUrl = null;

                XmlDocument doc = new XmlDocument();
                doc.Load("http://dl.hurhaar.com/gps/download.xml");

                WebClient client = new WebClient();

                if (type == doc.SelectSingleNode("//Files/dl[id='dll']/id").InnerText)
                {
                    dName = doc.SelectSingleNode("//Files/dl[id='dll']/name").InnerText;
                    dUrl = doc.SelectSingleNode("//Files/dl[id='dll']/url").InnerText;
                }

                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(dUrl), gpsPath + @"\" + dName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            double bytesIn = double.Parse(e.BytesReceived.ToString());
            double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
            double percentage = bytesIn / totalBytes * 100;
            label4.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
            progressBar1.Value = int.Parse(Math.Truncate(percentage).ToString());
        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            label4.Text = "File(s) downloaded";
            done++;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    //progressBar1.Value = 20;
                });
                string myConnection = "datasource=e28787-mysql.services.easyname.eu;port=3306;username=u28407db3;password=1992stifei17;";
                MySqlConnection myConn = new MySqlConnection(myConnection);

                string password = GetSHA1HashData(passwordText.Text);

                MySqlCommand SelectCommand = new MySqlCommand("select * from u28407db3.users where username='" + usernameText.Text + "' and password='" + password + "' ;", myConn);

                MySqlDataReader myReader;
                myConn.Open();
                myReader = SelectCommand.ExecuteReader();
                this.Invoke((MethodInvoker)delegate()
                {
                    //progressBar1.Value = 65;
                });
                int count = 0;
                while (myReader.Read())
                {
                    count = count + 1;
                }
                if (count == 1)
                {
                    this.Invoke((MethodInvoker)delegate()
                    {
                        //progressBar1.Value = 100;
                        Hide();
                    });
                    DialogResult = DialogResult.OK;
                }
                else if (count > 1)
                {
                    MessageBox.Show("Duplicate Username and password... Access denied");
                    this.Invoke((MethodInvoker)delegate()
                    {
                        //progressBar1.Value = 100;
                        button1.Enabled = true;
                        checkBox1.Enabled = true;
                        checkBox2.Enabled = true;
                    });
                }
                else
                {
                    MessageBox.Show("Username and/or password is not correct.. Please try again.");
                    this.Invoke((MethodInvoker)delegate()
                    {
                        //progressBar1.Value = 100;
                        button1.Enabled = true;
                        checkBox1.Enabled = true;
                        checkBox2.Enabled = true;
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Make sure you have an internet connection..." + Environment.NewLine + ex.Message);
                this.Invoke((MethodInvoker)delegate()
                {
                    //progressBar1.Value = 100;
                    button1.Enabled = true;
                    checkBox1.Enabled = true;
                    checkBox2.Enabled = true;
                });
            }
        }

        public void returnResult()
        {
            //return DialogResult.Yes;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked == true)
            {
                checkBox2.Enabled = true;
            }
            else
            {
                checkBox2.Enabled = false;
                checkBox2.Checked = false;
            }
        }

        private void usernameText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1.PerformClick();
            }
        }

        private void passwordText_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)13)
            {
                button1.PerformClick();
            }
        }

        public string returnUser()
        {
            String user = usernameText.Text;
            return user;
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }

    }
}
