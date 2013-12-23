using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using Ionic.Zip;

namespace GameProfileSaver
{
    public partial class Form1 : Form
    {

        static List<FileInfo> files = new List<FileInfo>();  // List that will hold the files and subfiles in path
        static List<DirectoryInfo> folders = new List<DirectoryInfo>(); // List that hold direcotries that cannot be accessed
        int gameID = 0;

        public Form1(string user)
        {
            InitializeComponent();
            currentUserLabel.Text = user;
        }

        public string returnUser()
        {
            String user = currentUserLabel.Text;
            return user;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            if (!Directory.Exists(gpsPath))
            {
                string userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

                Directory.CreateDirectory(gpsPath);
                if (!Directory.Exists(userDir))
                {
                    Directory.CreateDirectory(userDir);
                }
                if (!File.Exists(gpsPath + "/settings.xml"))
                {
                    createFile(gpsPath, "settings.xml");
                }
                if (!File.Exists(userDir + "/games.xml"))
                {
                    createFile(userDir, "games.xml");
                }
            }
            else
            {
                string userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

                if (!Directory.Exists(userDir))
                {
                    Directory.CreateDirectory(userDir);
                }
                if (!File.Exists(gpsPath + "/settings.xml"))
                {
                    createFile(gpsPath, "settings.xml");
                }
                if (!File.Exists(userDir + "/games.xml"))
                {
                    createFile(userDir, "games.xml");
                }
            }
            readGameList();
        }

        private void createFile(String path, String fileName)
        {
            XmlTextWriter xWriter = new XmlTextWriter(path + "/" + fileName, Encoding.UTF8);
            xWriter.Formatting = Formatting.Indented;
            xWriter.WriteStartElement("games");
            xWriter.WriteEndElement();
            xWriter.Close();
        }

        private void addText(String item)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "\\GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "\\games.xml");
            XmlNode fileToAdd = doc.CreateElement("file");
            String gName = comboBox1.SelectedItem.ToString();
            XmlNode createFiles = doc.SelectSingleNode("//games/game[gameName='" + gName + "']");
            XmlNode filesNode = doc.CreateElement("Files");

            if (doc.SelectSingleNode("//games/game[gameName='" + gName + "']/Files") == null)
            {
                createFiles.AppendChild(filesNode);
                doc.Save(userDir + "\\games.xml");
            }

            doc.Load(userDir + "\\games.xml");
            XmlNode gameName = doc.SelectSingleNode("//games/game[gameName='" + gName + "']/Files");

            fileToAdd.InnerText = item;
            gameName.AppendChild(fileToAdd);
            doc.Save(userDir + "\\games.xml");
        }

        private void addFilesFromFolder(List<string> files)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "\\GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "\\games.xml");
            String gName = comboBox1.SelectedItem.ToString();
            XmlNode createFiles = doc.SelectSingleNode("//games/game[gameName='" + gName + "']");
            XmlNode filesNode = doc.CreateElement("Files");

            if (doc.SelectSingleNode("//games/game[gameName='" + gName + "']/Files") ==null)
            {
                createFiles.AppendChild(filesNode);
                doc.Save(userDir + "\\games.xml");
            }

            doc.Load(userDir + "\\games.xml");
            XmlNode gameName = doc.SelectSingleNode("//games/game[gameName='" + gName + "']/Files");
            foreach (string f in files)
            {
                XmlNode fileToAdd = doc.CreateElement("file");
                fileToAdd.InnerText = f;
                listBox1.Items.Add(f);
                gameName.AppendChild(fileToAdd);
            }

            doc.Save(userDir + "\\games.xml");
            toolStripStatusLabel1.Text = "Files has been added...";
            toolStatusRefresh.Start();
        }

        private void addGame_Click(object sender, EventArgs e)
        {
            this.Hide();
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(openAddGameForm));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void openAddGameForm()
        {
            Application.Run(new AddGameForm(this));
        }

        private void readGameList()
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "/games.xml");
            foreach (XmlNode node in doc.SelectNodes("games/game"))
            {
                comboBox1.Items.Add(node.SelectSingleNode("gameName").InnerText);
            }
            try
            {
                comboBox1.SelectedIndex = 0;
            }
            catch (Exception ex)
            {

            }
        }

        private void addFiles_Click(object sender, EventArgs e)
        {
            openFileDialog1.ShowDialog();
            foreach (String file in openFileDialog1.FileNames)
            {
                addText(file);
                listBox1.Items.Add(file);
            }
        }

        public void comboAdd(object item)
        {
            comboBox1.Items.Add(item);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();

            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "\\GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "\\games.xml");
            String gName = comboBox1.SelectedItem.ToString();
            foreach (XmlNode node in doc.SelectNodes("//games/game[gameName='" + gName + "']/Files/file"))
            {
                try
                {
                    listBox1.Items.Add(node.InnerText);
                }catch(Exception ex)
                {
                    statusStrip1.Text = "No files to list for: " + gName;
                }
            }
        }

        public void refreshListBox()
        {
            listBox1.Items.Clear();

            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "\\GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "\\games.xml");
            String gName = comboBox1.SelectedItem.ToString();
            foreach (XmlNode node in doc.SelectNodes("//games/game[gameName='" + gName + "']/Files/file"))
            {
                try
                {
                    listBox1.Items.Add(node.InnerText);
                }
                catch (Exception ex)
                {
                    statusStrip1.Text = "No files to list for: " + gName;
                }
            }
        }

        private void addFolder_Click(object sender, EventArgs e)
        {
            DialogResult dr = folderBrowserDialog1.ShowDialog();
            if (dr == System.Windows.Forms.DialogResult.OK)
            {
                toolStripStatusLabel1.Text = "Adding files...";
                string folderName = folderBrowserDialog1.SelectedPath;
                addFilesFromFolder(DirSearch(folderName));
                toolStripStatusLabel1.Text = "Files added.";
            }
        }

        private List<String> DirSearch(string sDir)
        {
            List<String> files = new List<String>();
            try
            {
                foreach (string f in Directory.GetFiles(sDir))
                {
                    files.Add(f);
                }
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    files.AddRange(DirSearch(d));
                }
            }
            catch (System.Exception excpt)
            {
                MessageBox.Show(excpt.Message);
            }

            return files;
        }

        private void removeFile_Click(object sender, EventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            String gName = comboBox1.SelectedItem.ToString();
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "/games.xml");
            var xDoc = XDocument.Load(gpsPath + "/games.xml");

            foreach (string li in listBox1.SelectedItems)
            {
                string nodeToDelete = li;
                xDoc.Descendants()
                    .First(n => (string)n == nodeToDelete)
                    .Remove();
            }

            xDoc.Save(userDir + "/games.xml");
            refreshListBox();
        }

        private void startGame_Click(object sender, EventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "/games.xml");
            String gName = comboBox1.SelectedItem.ToString();
            XmlNode gameName = doc.SelectSingleNode("//games/game[gameName='" + gName + "']/exePath");
            String toStart = gameName.InnerText;
            toolStripStatusLabel1.Text = "Starting game: " + gName;
            Process game = Process.Start(toStart);
            gameRunningChecker.Start();
            gameRunningChecker.Interval = 2000;
            gameID = game.Id;
            toolStripStatusLabel1.Text = "Game in progress.";
        }

        private void gameRunningChecker_Tick(object sender, EventArgs e)
        {
            try
            {
                if (Process.GetProcessById(gameID).HasExited)
                {
                    toolStripStatusLabel1.Text = "Game closed";
                    gameRunningChecker.Stop();
                    toolStatusRefresh.Start();
                    backgroundWorker1.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                toolStripStatusLabel1.Text = "Game closed";
                gameRunningChecker.Stop();
                toolStatusRefresh.Start();
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void toolStatusRefresh_Tick(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = "Not doing anything";
            toolStatusRefresh.Stop();
        }

        private void openSettingsForm_Click(object sender, EventArgs e)
        {
            this.Hide();
            //Application.Run(new SettingsForm(this));
            System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(oppenSettingsForm));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
        }

        private void oppenSettingsForm()
        {
            Application.Run(new SettingsForm(this));
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void startSync_Click(object sender, EventArgs e)
        {
            backgroundWorker1.RunWorkerAsync();
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            String userDir = gpsPath + "/profiles/" + currentUserLabel.Text;

            XmlDocument doc = new XmlDocument();
            doc.Load(userDir + "\\games.xml");
            String gName = comboBox1.SelectedItem.ToString();
            ZipFile pack = new ZipFile();
            foreach (XmlNode node in doc.SelectNodes("//games/game[gameName='" + gName + "']/Files/file"))
            {
                try
                {
                    if (!Directory.Exists(userDir + "\\" + gName))
                    {
                        Directory.CreateDirectory(userDir + "\\" + gName);
                    }

                    pack.AddFile(node.InnerText);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            pack.Save(userDir + "\\" + gName);
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Application.Exit();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            String appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData).ToString();
            String gpsPath = appDataFolder + "/GameProfileSaver";
            String gName = comboBox1.SelectedItem.ToString();

            XmlDocument doc = new XmlDocument();
            doc.Load(gpsPath + "/games.xml");
            XmlNode toDelete = doc.SelectSingleNode("games/game[gameName='" + gName + "']");
            toDelete.ParentNode.RemoveChild(toDelete);
            doc.Save(gpsPath + "/games.xml");
            comboBox1.Items.Remove(gName);
            listBox1.Items.Clear();
        }

        private void logOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.check2 = false;
            Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void logOut()
        {
            Application.Run(new loginForm());
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (backgroundWorker1.IsBusy)
            {
                DialogResult dr = MessageBox.Show("Close it?", "You sure?", MessageBoxButtons.YesNo);
                if (dr == DialogResult.Yes)
                {

                }else if (dr == DialogResult.No)
                {
                    e.Cancel = false;
                }
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            toolStripProgressBar1.Value = e.ProgressPercentage;
        }
    }
}