﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using System.IO;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        Process cmd;
        JObject livingRoomObj;
        JObject kitchenObj;
        JObject drainObj;
        JObject foyerObj;
        JObject backyardObj;
        JObject basementObj;
        JObject bedroomObj;
        JObject jennyRoomObj;

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
            //General Initialization Zone (tm)
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

            if (true) 
            {

                //Actually pretty sick/goofy way of generating rando seed and making it a smaller number, should hold on to this and move it somewhere else later
                int randoSeed = 0;
                foreach (char c in seed.Text) 
                {
                    randoSeed += (int)c;
                }


                


                
                

                initializeStages();
                statusDialog.Text += "\nInitialized Stages";

                //Rewrite stage02 for testing
                JToken test1 = foyerObj.SelectToken("objects[336].object");
                test1.Replace("item_tamagotti");
                if (openUpstairs.Checked) 
                {
                    JToken latestToken = foyerObj.SelectToken("objects[512]");
                    latestToken.AddAfterSelf(Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../openUpstairs.json")) as JObject);                
                }
                
                File.WriteAllText("../../Stages/stage02.json", foyerObj.ToString());

                runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage02 " + @"D:\ChibiRando\Randomizer\Stages\stage02.json");


                statusDialog.Text += "\nUpdated Foyer Stage Data";

            }          
        }

        private void initializeStages() 
        {
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage07 -o " + @"D:\ChibiRando\Randomizer\Stages\stage07.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage01 -o " + @"D:\ChibiRando\Randomizer\Stages\stage01.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage11 -o " + @"D:\ChibiRando\Randomizer\Stages\stage11.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage09 -o " + @"D:\ChibiRando\Randomizer\Stages\stage09.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage03 -o " + @"D:\ChibiRando\Randomizer\Stages\stage03.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage06 -o " + @"D:\ChibiRando\Randomizer\Stages\stage06.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage04 -o " + @"D:\ChibiRando\Randomizer\Stages\stage04.json");
            runUnplugCommand("stage export --iso " + isoFilePath.Text + " stage02 -o " + @"D:\ChibiRando\Randomizer\Stages\stage02.json");

            livingRoomObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage07.json")) as JObject;
            kitchenObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage01.json")) as JObject;
            drainObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage11.json")) as JObject;
            backyardObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage09.json")) as JObject;
            basementObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage03.json")) as JObject;
            bedroomObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage06.json")) as JObject;
            jennyRoomObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage04.json")) as JObject;
            foyerObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../Stages/stage02.json")) as JObject;

            
        }
        private void runUnplugCommand(string command) 
        {
            var info = new ProcessStartInfo();

            string fullCommand = @"D:\ChibiRando\Randomizer\unplug " + command;

            info.UseShellExecute = false;
            info.WorkingDirectory = @"C:\Windows\System32";

            info.FileName = "cmd.exe";
            info.Verb = "runas";
            info.Arguments = "/C " + fullCommand;
            info.WindowStyle = ProcessWindowStyle.Minimized;
            info.RedirectStandardOutput = true;
            cmd.StartInfo = info;
            cmd.Start();

            

            statusDialog.Text += "\nRunning command: " + fullCommand;

            statusDialog.Text += "\n" + cmd.StandardOutput.ReadToEnd();
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
