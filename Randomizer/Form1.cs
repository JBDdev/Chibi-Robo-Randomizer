using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Process cmd;
        public Form1()
        {
            InitializeComponent();
        }

        private void openISO_Click(object sender, EventArgs e)
        {


            using (OpenFileDialog ofd = new OpenFileDialog()) 
            {
                ofd.InitialDirectory = "c:\\";
                ofd.Filter = "ISO File (*.iso)|*.iso";
                ofd.RestoreDirectory = true;
                if (ofd.ShowDialog() == DialogResult.OK)
                {
                    isoFilePath.Text = ofd.FileName;
                }
            }
            
            
        }

        private void openDestination_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog()) 
            {
                if (dialog.ShowDialog() == DialogResult.OK) 
                {
                    destinationPath.Text = dialog.SelectedPath;
                }
            }
        }

        private void isoFilePath_TextChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            Random r = new Random();
            for (int i = 0; i < 10; i++) 
            {
                seed.Text += (char)r.Next(33, 126);
            }

            cmd = new Process();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void settingsLabel_Click(object sender, EventArgs e)
        {

        }

        private void seed_Click(object sender, EventArgs e)
        {

        }

        private void randomizeButton_Click(object sender, EventArgs e)
        {

            if (validInput()) 
            {

                //Actually pretty sick way of generating rando seed and making it a smaller number, should hold on to this and move it somewhere else later
                int randoSeed = 0;
                foreach (char c in seed.Text) 
                {
                    randoSeed += (int)c;
                }

                runUnplugCommand("help");
                
                //statusDialog.Text += "\nSucessfully generated randomized ISO at " + destinationPath.Text + " using seed " + randoSeed;
            }


            
        }

        private void runUnplugCommand(string command) 
        {
            var info = new ProcessStartInfo();

            string fullCommand = @"D:\ChibiRando\Randomizer\unplug " + command;

            info.UseShellExecute = false;
            info.WorkingDirectory = @"C:\Windows\System32";

            info.FileName = "cmd.exe";
            info.Verb = "runas";
            info.Arguments = "/c " + command;
            info.WindowStyle = ProcessWindowStyle.Minimized;
            info.RedirectStandardOutput = true;
            cmd.StartInfo = info;
            cmd.Start();

            statusDialog.Text += cmd.StandardOutput.ReadToEnd();
        }

        private bool validInput()
        {
            statusDialog.Text = "";
            //Input Validation
            string isoValidation = isoFilePath.Text;
            isoValidation = isoValidation.Substring(isoValidation.Length - 4);

            if (isoValidation.ToLower() == ".iso")
            {
                statusDialog.Text += "Validated ISO";
            }
            else
            {
                statusDialog.Text += "[ERROR] Invalid file path to Chibi-Robo ISO";
                return false;
            }

            if (destinationPath.Text != "<- Set destination path")
            {
                statusDialog.Text += "\nValidated file path";
            }
            else
            {
                statusDialog.Text += "\n[ERROR] Invalid destination file path";
                return false;
            }

            if (logicSettings.SelectedItem != null)
            {
                statusDialog.Text += "\nValidation complete";
            }
            else 
            {
                statusDialog.Text += "\n[ERROR] Please select a game mode from the dropdown menu.";
                return false;
            }

            return true;
        }
    }
}
