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

        RootObject stageData;
        ItemPool itemPool;
        Random r;

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

            occupiedChecks = new List<int>();
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

                //Any code that handles stage editing / randomization goes here!
                //
                //
                //
                //

                stageData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(File.ReadAllText("../../itemChecks.json"));
                itemPool = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemPool>(File.ReadAllText("../../itemPool.json"));

                r = new Random(randoSeed);
                shuffleItemsGlitchless();


                //Edits for Open Upstairs setting
                if (openUpstairs.Checked)
                {
                    JToken latestToken = foyerObj.SelectToken("objects[512]");
                    latestToken.AddAfterSelf(Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("../../openUpstairs.json")) as JObject);
                }

                reimportStages();

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

            //statusDialog.Text += "\nRunning command: " + fullCommand;

            //statusDialog.Text += "\n" + cmd.StandardOutput.ReadToEnd();
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

        //Picks locations for the items, puts them into the appropriate locations, and then updates the spoiler log
        private void shuffleItemsGlitchless() 
        {
            //Setup
            int nextCheck;
            List<ItemLocation> allLocations = new List<ItemLocation>();
            List<bool> occupiedChecks = new List<bool>();
            for (int i = 0; i < stageData.rooms.Count; i++) 
            {
                for (int j = 0; j < stageData.rooms[i].locations.Count(); i++) 
                {
                    allLocations.Add(stageData.rooms[i].locations[j]);
                    occupiedChecks.Add(false);
                }                    
            }

            //Shuffle Charger and Battery
            while (true) 
            {
                nextCheck = r.Next(0, allLocations.Count() - 1);
                if (allLocations[nextCheck].Prereqs.Contains("ladder") || allLocations[nextCheck].Prereqs.Contains("bridge")) 
                {
                    continue;
                }
                //Edit the correct object
            }

            //Check here for any locks, then check again after shuffling battery
            

            
            //Shuffle Leg

            //Shuffle any key items that would otherwise cause locks for the above


        }


        private void reimportStages() 
        {
            File.WriteAllText("../../Stages/stage01.json", kitchenObj.ToString());
            File.WriteAllText("../../Stages/stage02.json", foyerObj.ToString());
            File.WriteAllText("../../Stages/stage03.json", basementObj.ToString());
            File.WriteAllText("../../Stages/stage04.json", jennyRoomObj.ToString());
            File.WriteAllText("../../Stages/stage06.json", bedroomObj.ToString());
            File.WriteAllText("../../Stages/stage07.json", livingRoomObj.ToString());
            File.WriteAllText("../../Stages/stage09.json", backyardObj.ToString());
            File.WriteAllText("../../Stages/stage11.json", drainObj.ToString());

            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage01 " + @"D:\ChibiRando\Randomizer\Stages\stage01.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage02 " + @"D:\ChibiRando\Randomizer\Stages\stage02.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage03 " + @"D:\ChibiRando\Randomizer\Stages\stage03.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage04 " + @"D:\ChibiRando\Randomizer\Stages\stage04.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage06 " + @"D:\ChibiRando\Randomizer\Stages\stage06.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage07 " + @"D:\ChibiRando\Randomizer\Stages\stage07.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage09 " + @"D:\ChibiRando\Randomizer\Stages\stage09.json");
            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage11 " + @"D:\ChibiRando\Randomizer\Stages\stage11.json");
        }

        private void testCodeDump() 
        {
 
            //This is some code to pick a random check from the living room and put a frog in its place
            RootObject test = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(File.ReadAllText("../../itemChecks.json"));

            Random r = new Random();
            int testLocation = r.Next(0, test.rooms[0].locations.Count() - 1);


            JToken test1 = livingRoomObj.SelectToken("objects[" + test.rooms[0].locations[testLocation].ID + "].object");
            test1.Replace("item_frog");

            File.WriteAllText("../../Stages/stage07.json", livingRoomObj.ToString());

            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage07 " + @"D:\ChibiRando\Randomizer\Stages\stage07.json");




            //Rewriting foyer + code for adding in upstairs early
            JToken test2 = foyerObj.SelectToken("objects[336].object");
            test2.Replace("item_tamagotti");
            

            File.WriteAllText("../../Stages/stage02.json", foyerObj.ToString());

            runUnplugCommand("stage import --iso " + isoFilePath.Text + " stage02 " + @"D:\ChibiRando\Randomizer\Stages\stage02.json");


            //Replacing the living room happy blocks with frogs

            string[] flags = { "flash", "spawn", "cull", "lift", "interact" };
            foreach (ItemLocation loc in test.rooms[0].locations)
            {
                if (loc.ID < 240 || loc.ID > 249)
                    continue;
                livingRoomObj.SelectToken("objects[" + loc.ID + "].object").Replace("item_frog");

                int finalFlagIndex = livingRoomObj.SelectToken("objects[" + loc.ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in livingRoomObj.SelectToken("objects[" + loc.ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                livingRoomObj.SelectToken("objects[" + loc.ID + "].flags[0]").AddAfterSelf("flash");
                livingRoomObj.SelectToken("objects[" + loc.ID + "].flags[0]").AddAfterSelf("cull");
                livingRoomObj.SelectToken("objects[" + loc.ID + "].flags[0]").AddAfterSelf("lift");
                livingRoomObj.SelectToken("objects[" + loc.ID + "].flags[0]").AddAfterSelf("interact");

            }

        }
    }
}
