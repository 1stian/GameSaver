using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace GameProfileSaver
{
    public partial class AddGameForm : Form
    {
        Form1 form1;
        
        public AddGameForm(Form1 frm)
        {
            InitializeComponent();
            form1 = frm;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Executable (*.exe)|*.exe";
            Invoke((Action)(() => { openFileDialog1.ShowDialog(); }));
            textBox2.Text = openFileDialog1.FileName;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "\\GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + form1.returnUser();

            if (textBox1.Text == "")
            {
                MessageBox.Show("Please give it a name.");
            }
            else if (textBox2.Text == "No path given")
            {
                MessageBox.Show("Please chose the exe path.");
            }
            else
            {
                String nameForGame = textBox1.Text;
                String toFile = nameForGame.Replace(" ", string.Empty);

                XmlDocument doc = new XmlDocument();
                doc.Load(userDir + "\\games.xml");
                XmlNode game = doc.CreateElement("game");
                XmlNode gameName = doc.CreateElement("gameName");
                XmlNode exePath = doc.CreateElement("exePath");
                XmlNode files = doc.CreateElement("Files");
                game.AppendChild(gameName);
                gameName.InnerText = textBox1.Text;
                game.AppendChild(exePath);
                exePath.InnerText = textBox2.Text;
                game.AppendChild(files);
                doc.DocumentElement.AppendChild(game);
                doc.Save(userDir + "\\games.xml");

                this.Invoke((MethodInvoker)delegate()
                {
                    form1.comboAdd(textBox1.Text);
                });

                this.Close();
            }
        }

        private void AddGameForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Invoke((MethodInvoker)delegate()
            {
                form1.Show();
            });
        }

        private void AddGameForm_Load(object sender, EventArgs e)
        {

        }
    }
}
