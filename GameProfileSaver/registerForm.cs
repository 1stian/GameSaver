using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using System.Security.Cryptography;

namespace GameProfileSaver
{
    public partial class registerForm : Form
    {

        bool succeed;

        private loginForm form1;
        public registerForm(loginForm frm)
        {
            InitializeComponent();
            form1 = frm;
        }

        private void registerForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                form1.Show();
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (usernameText.Text == "")
            {
                MessageBox.Show("Please enter a username!");
            }
            else if (passwordText.Text == "")
            {
                MessageBox.Show("Please enter a password!");
            }
            else
            {
                usernameText.Enabled = false;
                passwordText.Enabled = false;
                emailText.Enabled = false;
                button1.Enabled = false;
                backgroundWorker1.RunWorkerAsync();
                progressBar1.Value = 20;
            }
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

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            try
            {
                string myConnection = "datasource=e28787-mysql.services.easyname.eu;port=3306;username=u28407db3;password=1992stifei17;";
                MySqlConnection myConn = new MySqlConnection(myConnection);

                string password = GetSHA1HashData(passwordText.Text);

                MySqlCommand SelectCommand = new MySqlCommand("select * from u28407db3.users where username='" + usernameText.Text + "' ;", myConn);

                MySqlDataReader myReader;
                myConn.Open();
                myReader = SelectCommand.ExecuteReader();
                this.Invoke((MethodInvoker)delegate()
                {
                    
                });
                int count = 0;
                while (myReader.Read())
                {
                    count = count + 1;
                }
                if (count == 1)
                {
                    MessageBox.Show("You're already registered. Please login instead..");
                    this.Invoke((MethodInvoker)delegate()
                    {
                        usernameText.Enabled = true;
                        passwordText.Enabled = true;
                        emailText.Enabled = true;
                        button1.Enabled = true;
                        succeed = false;
                    });
                }
                else if (count == 0)
                {
                    myConn.Close();
                    MySqlCommand toInsert = new MySqlCommand();
                    toInsert.Connection = myConn;
                    toInsert.CommandText = "INSERT INTO u28407db3.users(username,password,email) VALUES(?username,?password,?email)";
                    toInsert.Parameters.Add("?username", MySqlDbType.VarChar).Value = usernameText.Text;
                    toInsert.Parameters.Add("?password", MySqlDbType.VarChar).Value = password;
                    toInsert.Parameters.Add("?email", MySqlDbType.VarChar).Value = emailText.Text;
                    this.Invoke((MethodInvoker)delegate()
                    {
                        progressBar1.Value = 40;
                    });
                    myConn.Open();
                    this.Invoke((MethodInvoker)delegate()
                    {
                        progressBar1.Value = 60;
                    });
                    toInsert.ExecuteNonQuery();
                    this.Invoke((MethodInvoker)delegate()
                    {
                        progressBar1.Value = 90;
                    });
                    succeed = true;
                }
                myConn.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error! Please report this error to the developer!" + Environment.NewLine + ex.Message);
                this.Invoke((MethodInvoker)delegate()
                {
                    usernameText.Enabled = true;
                    passwordText.Enabled = true;
                    emailText.Enabled = true;
                    button1.Enabled = true;
                    succeed = false;
                });               
            }
        }

        private void registerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                DialogResult dr = MessageBox.Show("You sure you want to close this while the registration progress is running? It will be aborted", "You sure?", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {
                    Close();
                }
                else if (dr == DialogResult.No)
                {
                    e.Cancel = true;
                }
            }
        }

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (succeed)
            {
                MessageBox.Show("You have now been registered");
                this.Invoke((MethodInvoker)delegate()
                {
                    progressBar1.Value = 100;
                    Close();
                });
            }
            else
            {
                this.Invoke((MethodInvoker)delegate()
                {
                    usernameText.Enabled = true;
                    passwordText.Enabled = true;
                    emailText.Enabled = true;
                    button1.Enabled = true;
                }); 
            }
        }
    }
}
