using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace CopyAgent_v1
{
    public partial class Form1 : Form
    {
        private string _source_input { get; set; }
        private string _source_output { get; set; }
        DateTime startTimeProgram { get; set; }
        TimeSpan timeSpan { get; set; }
        private BackgroundWorker bw;
        private int maxbytes;
        private int total;
        private int copied;
        private bool isBlock;
        private bool isCancel;
        int CopySucces = 1;

        List<FileInfo> listPathsFileInfo { get; set; }

        public Form1()
        {
            isBlock = bool.Parse(ConfigurationManager.AppSettings.Get("mierzep"));

            if (bw != null)
            {
                bw.Dispose();
            }

            bw = new BackgroundWorker();
            listPathsFileInfo = new List<FileInfo>();
            startTimeProgram = DateTime.Now;
            InitializeComponent();
            timer_program.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        private void RefreshTextBox()
        {
            textBox_Input.Text = _source_input;
            textBox_Output.Text = _source_output;
        }

        private void btn_imput_Click(object sender, EventArgs e)
        {
            GetSource(Type.Input);

            RefreshTextBox();

            StartProcess(); 
        }

        private void btn_Output_Click(object sender, EventArgs e)
        {
            GetSource(Type.Output);

            RefreshTextBox();
        }
        
        private void button_Start_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(_source_input))
            {
                MessageBox.Show("Proszę podać scieżkę WEJŚCIA!", "Input Empty!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (string.IsNullOrWhiteSpace(_source_output))
            {
                MessageBox.Show("Proszę podać scieżkę WYJŚCIA!", "Output Empty!", MessageBoxButtons.OK, MessageBoxIcon.Error);

                return;
            }

            bw.DoWork += bw_DoWork;
            bw.ProgressChanged += bw_ProgressChanged;
            bw.RunWorkerCompleted += bw_RunWorkerCompleted;
            bw.WorkerSupportsCancellation = true;

            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            if (listPathsFileInfo.Count == 0)
            {
                MessageBox.Show("Brak plików w katalogach", "Warning!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            foreach (FileInfo item in listPathsFileInfo)
            {
                if (isCancel == true)
                {
                    return;
                }

                Copy(item, _source_output);
            }
        }

        public void Copy(FileInfo fileSource, string targetDirectory)
        {
            DirectoryInfo diTarget = new DirectoryInfo(targetDirectory);
            //Gets size of all files present in source folder.
            GetSize(fileSource, diTarget);
            maxbytes = maxbytes / 1024;

            progressBar1.BeginInvoke((Action)(() =>
            {
                progressBar1.Maximum = maxbytes;
            }));

            CopyAll(fileSource, diTarget);
        }

        public void CopyAll(FileInfo fi, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            try
            {
                fi.CopyTo(Path.Combine(target.ToString(), fi.Name), true);

                total += (int)fi.Length;

                copied += (int)fi.Length;
                copied /= 1024;

                SetLog(fi.Name, "OK");
            }
            catch (Exception ex)
            {
                SetLog(fi.Name + ex.Message, "False");
            }

            ReloadAndCheck();
        }

        public void GetSize(FileInfo fi, DirectoryInfo target)
        {
            if (Directory.Exists(target.FullName) == false)
            {
                Directory.CreateDirectory(target.FullName);
            }

            maxbytes += (int)fi.Length;//Size of File
        }

        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            progressBar1.Visible = true;
            progressBar1.Step = copied;

            progressBar1.PerformStep();
        }
                                                  
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {

            MessageBox.Show("Zakończone przenoszenie", "Completed!", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void StartProcess()
        {
            treeView_Input.Nodes.Clear();
            DirectoryInfo directoryInfo = new DirectoryInfo(_source_input);
            treeView_Input.Nodes.Add(CreateDirectoryNode(directoryInfo));
            treeView_Input.TopNode.ExpandAll();
            labelStatusCount.Text = listPathsFileInfo.Count.ToString();
        }

        public TreeNode CreateDirectoryNode(DirectoryInfo directoryInfo)
        {
            TreeNode treeNode = new TreeNode(directoryInfo.Name);
            foreach (var directory in directoryInfo.GetDirectories())
                treeNode.Nodes.Add(CreateDirectoryNode(directory));

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                treeNode.Nodes.Add(new TreeNode(file.Name));
                listPathsFileInfo.Add(file);
            }

            return treeNode;
        }

        void ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            labelProgress.Text = string.Format("Result {0}: Percent {1}", e.UserState, e.ProgressPercentage);
        }

        void RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (isCancel)
            {

            }

            if (e.Cancelled)
            {
                MessageBox.Show("cancel");
            }
            else if (e.Error != null)
            {
                MessageBox.Show("error");
            }
        }

        private void timer_program_Tick(object sender, EventArgs e)
        {
            SetProgramTime();

            if (isBlock && (timeSpan.Seconds >= 15 || timeSpan.Seconds == 16))
            {
                timer_program.Stop();
                if (MessageBox.Show("Your license has expired... \n\n Contact: m.mierzejewskipatryk@gmail.com", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information) == DialogResult.OK)
                {
                    Application.Exit();
                }
                else
                {
                    Application.Exit();
                }
            }
        }
        private void SetProgramTime()
        {
            timeSpan = new TimeSpan(
                (DateTime.Now - startTimeProgram).Hours,
                (DateTime.Now - startTimeProgram).Minutes,
                (DateTime.Now - startTimeProgram).Seconds);

            labelTime.Text = timeSpan.ToString();
        }

        private void ReloadAndCheck()
        {
            progressBar1.BeginInvoke((Action)(() =>
            {
                progressBar1.Visible = true;
                progressBar1.Step = copied;

                progressBar1.PerformStep();

            }));

            labelProgress.BeginInvoke((Action)(() =>
            {
                labelProgress.Visible = true;
                labelProgress.Text = (total / 1048576).ToString() + "MB of " + (maxbytes / 1024).ToString() + "MB copied";

                labelProgress.Refresh();
            }));
        }

        private void SetLog(string name, string status)
        {
            if (status.Equals("OK"))
            {
                listBox_status.BeginInvoke((Action)(() =>
                {
                    listBox_status.Items.Add($"Skopiowano - {name}");
                }));

                labelStatusCount.BeginInvoke((Action)(() =>
                {
                    labelStatusCount.Text = $"{listPathsFileInfo.Count} - {CopySucces++}";
                }));
            }
            else
            {
                listBox_status.BeginInvoke((Action)(() =>
                {
                    listBox_status.ForeColor = Color.Red;
                    listBox_status.Items.Add($"ERROR - {name}");
                }));
            }
            
        }

        private void GetSource(Type type)
        {
            using (var fbd = new FolderBrowserDialog())
            {
                DialogResult result = fbd.ShowDialog();

                if (result == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    if (type == Type.Input)
                    {
                        _source_input = fbd.SelectedPath;
                    }
                    else if (type == Type.Output)
                    {
                        _source_output = fbd.SelectedPath;
                    }
                    else
                    {

                    }
                }
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            if (bw.IsBusy != false)
            {
                bw.CancelAsync();
                buttonCancel.Enabled = false;
            }
            
            isCancel = true;
        }

        private void tabPage_About_Click(object sender, EventArgs e)
        {

        }

        private enum Type
        {
            Input,
            Output
        }
    }
}
