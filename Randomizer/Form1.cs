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
using System.Reflection;
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        
        JObject livingRoomObj;
        JObject kitchenObj;
        JObject drainObj;
        JObject foyerObj;
        JObject backyardObj;
        JObject basementObj;
        JObject bedroomObj;
        JObject jennyRoomObj;
        JObject shopObj;
        JObject globals;

        RootObject stageData;
        ItemPool itemPool;
        Random r;
        string newIsoPath;
        int newPassword;

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
            //Load Resources

            //General Initialization Zone (tm)
            Random r = new Random();
            for (int i = 0; i < 10; i++) 
            {
                seed.Text += (char)r.Next(33, 126);
            }
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

        //This is where the magic happens!
        private void randomizeButton_Click(object sender, EventArgs e)
        {
            if (validInput())
            {
                //Takes each character of the string and casts it to an int, then adds each together to create a integer representation of the seed
                int randoSeed = 0;

                foreach (char c in seed.Text)
                {
                    randoSeed += (int)c;
                }

                newPassword = 200667;

                newIsoPath = destinationPath.Text + "\\chibiRando_" + randoSeed + ".iso";
                
                File.Copy(isoFilePath.Text, newIsoPath, true);

                initializeStages();


                //Any code that handles stage editing / randomization goes here!


                Stream stageDataStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChibiRoboRando.itemChecks.json");
                StreamReader readStageData = new StreamReader(stageDataStream);
                Stream itemPoolStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChibiRoboRando.itemPool.json");
                StreamReader readItemPool = new StreamReader(itemPoolStream);

                stageData = Newtonsoft.Json.JsonConvert.DeserializeObject<RootObject>(readStageData.ReadToEnd());
                itemPool = Newtonsoft.Json.JsonConvert.DeserializeObject<ItemPool>(readItemPool.ReadToEnd());

                r = new Random(randoSeed);

                //Performs the randomization based on the settings
                Dictionary<string, string> newSpoilerLog = shuffleItemsGlitchless();

                //Settings involving edits to globals

                //Turns off Chibi Vision for all objects if enabled
                if (chibiVision.Checked) 
                {
                    for (int i = 0; i < 159; i++)
                    {
                        globals.SelectToken("items[" + i + "].flags.chibiVision").Replace(false);
                    }
                }

                //Randomizes foot passcode, if enabled
                if (passwordRando.Checked) 
                {
                    newPassword = r.Next(100000, 999999);
                    globals.SelectToken("stats[12].name").Replace("Password: " + newPassword);
                    globals.SelectToken("items[124].description").Replace("The number \"" + newPassword + "\" is engraved\non the inside.");
                }


                //Add PJs to Shop if enabled
                if (freePJ.Checked) 
                {
                    JToken unusedShopItem = shopObj.SelectToken("items[16]");
                    unusedShopItem.SelectToken("item").Replace("pajamas");
                    unusedShopItem.SelectToken("price").Replace(10);
                    unusedShopItem.SelectToken("limit").Replace(1);
                }


                //Edits for Open Downstairs
                if (freePJ.Checked)
                {
                    JToken unusedShopItem = shopObj.SelectToken("items[17]");
                    unusedShopItem.SelectToken("item").Replace("drake_redcrest_suit");
                    unusedShopItem.SelectToken("price").Replace(10);
                    unusedShopItem.SelectToken("limit").Replace(1);
                }


                //Edits for Open Upstairs setting
                if (openUpstairs.Checked)
                {
                    JToken latestToken = foyerObj.SelectToken("objects[512]");

                    Stream openUpstairsStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("ChibiRoboRando.openUpstairs.json");
                    StreamReader readOpenUpstairs = new StreamReader(openUpstairsStream);
                    latestToken.AddAfterSelf(Newtonsoft.Json.JsonConvert.DeserializeObject(readOpenUpstairs.ReadToEnd()) as JObject);
                }
              

                //Spoiler log output
                using (StreamWriter logOutput = new StreamWriter(File.OpenWrite(destinationPath.Text + "\\Spoiler Log " + randoSeed + ".txt")))
                {
                    logOutput.WriteLine("******");
                    logOutput.WriteLine("Seed: " + seed.Text);
                    logOutput.WriteLine("Mode: " + logicSettings.Text);
                    logOutput.WriteLine("Password:  " + newPassword);
                    logOutput.WriteLine("Open Upstairs: " + openUpstairs.Checked);
                    logOutput.WriteLine("Charged Battery: " + batteryCharge.Checked);
                    logOutput.WriteLine("Free PJs: " + freePJ.Checked);
                    logOutput.WriteLine("Open Downstairs: " + openDownstairs.Checked);
                    logOutput.WriteLine("Randomize Password: " + passwordRando.Checked);
                    logOutput.WriteLine("Chibi-Vision Off: " + chibiVision.Checked);
                    logOutput.WriteLine("******\n");
                    logOutput.WriteLine("Locations: \n");

                    foreach (string key in newSpoilerLog.Keys)
                    {
                        logOutput.WriteLine(key + ": " + newSpoilerLog[key]);
                    }
                }

                reimportStages();

                statusDialog.Text += "\nISO Rebuilding Complete :)";
            }
        }

        private void initializeStages() 
        {
            List<string> oldFiles = new List<string>();
            foreach (string f in Directory.GetFiles(Directory.GetCurrentDirectory())) 
            {
                if (f.Substring(0,4) == "stage")
                {
                    oldFiles.Add(f);
                }
            }
            foreach (string f in oldFiles) 
            {
                File.Delete(f);
            }

            //statusDialog.Text += "stage export --iso \"" + newIsoPath + "\" stage07 -o " + Directory.GetCurrentDirectory() + @"stage07.json";
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage07 -o \"" + Directory.GetCurrentDirectory() + @"\stage07.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage01 -o \"" + Directory.GetCurrentDirectory() + @"\stage01.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage11 -o \"" + Directory.GetCurrentDirectory() + @"\stage11.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage09 -o \"" + Directory.GetCurrentDirectory() + @"\stage09.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage03 -o \"" + Directory.GetCurrentDirectory() + @"\stage03.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage06 -o \"" + Directory.GetCurrentDirectory() + @"\stage06.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage04 -o \"" + Directory.GetCurrentDirectory() + @"\stage04.json" + "\"");
            runUnplugCommand("stage export --iso \"" + newIsoPath + "\" stage02 -o \"" + Directory.GetCurrentDirectory() + @"\stage02.json" + "\"");

            runUnplugCommand("globals export --iso \"" + newIsoPath + "\" -o \"" + Directory.GetCurrentDirectory() + @"\globals.json");

            runUnplugCommand("shop export --iso \"" + newIsoPath + "\" -o \"" + Directory.GetCurrentDirectory() + @"\shop.json" + "\"");

            globals = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("globals.json")) as JObject;

            livingRoomObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage07.json")) as JObject;
            kitchenObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage01.json")) as JObject;
            drainObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage11.json")) as JObject;
            backyardObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage09.json")) as JObject;
            basementObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage03.json")) as JObject;
            bedroomObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage06.json")) as JObject;
            jennyRoomObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage04.json")) as JObject;
            foyerObj = Newtonsoft.Json.JsonConvert.DeserializeObject(File.ReadAllText("stage02.json")) as JObject;

            string shopInput = File.ReadAllText("shop.json");
            shopObj = Newtonsoft.Json.JsonConvert.DeserializeObject(@"{ 'items': " + shopInput + "}") as JObject;


        }
        private void runUnplugCommand(string command) 
        {
            using (Process cmd = new Process())
            {
                ////CD to swap to 

                //ProcessStartInfo cdCommandInfo = new ProcessStartInfo();

                //cdCommandInfo.UseShellExecute = false;
                //cdCommandInfo.WorkingDirectory = @"C:\Windows\System32";

                //cdCommandInfo.FileName = "cmd.exe";
                //cdCommandInfo.Verb = "runas";
                //cdCommandInfo.Arguments = "/C cd \"" + Directory.GetCurrentDirectory() + "\""; ;
                //cdCommandInfo.WindowStyle = ProcessWindowStyle.Minimized;
                //cdCommandInfo.RedirectStandardOutput = true;
                //cmd.StartInfo = cdCommandInfo;
                //cmd.Start();


                ProcessStartInfo unplugCommandInfo = new ProcessStartInfo();
                
                //string fullCommand = Directory.GetCurrentDirectory() + "" + command;
                string fullCommand = "" + Directory.GetCurrentDirectory() + "\\unplug.exe " + command;

                unplugCommandInfo.UseShellExecute = false;
                unplugCommandInfo.WorkingDirectory = @"C:\Windows\System32";

                unplugCommandInfo.FileName = "cmd.exe";
                unplugCommandInfo.Verb = "runas";
                unplugCommandInfo.Arguments = "/C " + fullCommand;
                unplugCommandInfo.WindowStyle = ProcessWindowStyle.Minimized;
                unplugCommandInfo.RedirectStandardOutput = true;
                cmd.StartInfo = unplugCommandInfo;
                cmd.Start();

                //statusDialog.Text += "\nFull Command: " + fullCommand;
                //statusDialog.Text += "\nArguments put into CMD: " + unplugCommandInfo.Arguments;
                
                StreamReader sr = cmd.StandardOutput;
                string test = sr.ReadToEnd();
                //statusDialog.Text += "\n" + sr.ReadToEnd();
                cmd.WaitForExit();
            }           
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

        //Picks locations for the items, puts them into the appropriate locations, and then returns the spoiler log
        private Dictionary<string, string> shuffleItemsGlitchless() 
        {
            //*** SETUP ***

            //List of all checks
            List<ItemLocation> allLocations = new List<ItemLocation>();

            //Tracks what checks are occupied
            List<bool> occupiedChecks = new List<bool>();

            //This will let us build the spoiler log later!
            Dictionary<string, string> spoilerLog = new Dictionary<string, string>();

            ItemLocation chargerLocation = null;
            ItemLocation batteryLocation = null;
            ItemLocation legLocation;

            Stopwatch stopwatch = new Stopwatch();


            //Builds the list of checks and occupiedChecks counter
            for (int i = 0; i < stageData.rooms.Count; i++) 
            {
                for (int j = 0; j < stageData.rooms[i].locations.Count(); j++) 
                {
                    allLocations.Add(stageData.rooms[i].locations[j]);
                    occupiedChecks.Add(false);
                }                    
            }


            //Clears all key item checks and replaces them with coin_c objects
            foreach (ItemLocation location in allLocations) 
            {
                if (allLocations.IndexOf(location) > (allLocations.Count() - stageData.rooms[8].locations.Count()))
                {
                    break;
                }
                for (int i = 0; i < itemPool.Items.Count - 8; i++) 
                {
                    if (location.ObjectName == itemPool.Items[i].objectName) 
                    {
                        insertItem("item_kami_kuzu", allLocations.IndexOf(location));
                        break;
                    }
                }
            }
            //Flattens out shop checks with null values
            shopObj.SelectToken("items[3].item").Replace(null);
            shopObj.SelectToken("items[4].item").Replace(null);
            shopObj.SelectToken("items[5].item").Replace(null);
            shopObj.SelectToken("items[6].item").Replace(null);
            shopObj.SelectToken("items[7].item").Replace(null);
            shopObj.SelectToken("items[9].item").Replace(null);
            shopObj.SelectToken("items[10].item").Replace(null);
            shopObj.SelectToken("items[11].item").Replace(null);
            shopObj.SelectToken("items[12].item").Replace(null);
            shopObj.SelectToken("items[13].item").Replace(null);
            shopObj.SelectToken("items[14].item").Replace(null);
            shopObj.SelectToken("items[15].item").Replace(null);

            #region Old Implementation
            stopwatch.Start();
            #region Junk Items

            for (int i = 0; i < 28; i++)
            {
                spoilerLog.Add("10M Coin " + (i + 1), allLocations[shuffleItem("coin_c", occupiedChecks, new string[] { "shop" }, allLocations)].Description);
            }

            for (int i = 0; i < 7; i++)
            {
                spoilerLog.Add("50M Coin " + (i + 1), allLocations[shuffleItem("coin_s", occupiedChecks, new string[] { "shop" }, allLocations)].Description);
            }

            for (int i = 0; i < 4; i++)
            {
                spoilerLog.Add("100M Coin " + (i + 1), allLocations[shuffleItem("coin_g", occupiedChecks, new string[] { "shop" }, allLocations)].Description);
            }

            for (int i = 0; i < 15; i++) 
            {
                spoilerLog.Add("Happy Block " + (i + 1), allLocations[shuffleItem("living_happy_box", occupiedChecks, new string[] { "shop" }, allLocations)].Description);
            }
            #endregion

            #region Key Item Important Checks
            //Shuffles Charger
            while (true)
            {
                int nextCheck = r.Next(0, allLocations.Count() - 1);
                if (openUpstairs.Checked)
                {
                    if (!validLocation(nextCheck, new string[] { "divorce" }, allLocations))
                    {

                    }
                    else
                    {
                        chargerLocation = allLocations[nextCheck];
                        occupiedChecks[nextCheck] = true;
                        insertItem("item_chibi_house_denti_2", nextCheck);
                        spoilerLog.Add("Giga-Charger", allLocations[nextCheck].Description);
                        break;
                    }
                }
                else 
                {
                    if (!validLocation(nextCheck, new string[] { "ladder", "bridge", "divorce" }, allLocations))
                    {

                    }
                    else
                    {
                        chargerLocation = allLocations[nextCheck];
                        occupiedChecks[nextCheck] = true;
                        insertItem("item_chibi_house_denti_2", nextCheck);
                        spoilerLog.Add("Giga-Charger", allLocations[nextCheck].Description);
                        break;
                    }
                }
            }
            
            //Shuffles Uncharged Battery
            while (true)
            {                
                int nextCheck = r.Next(0, allLocations.Count() - 1);

                if (openUpstairs.Checked)
                {
                    if (occupiedChecks[nextCheck] == true || !validLocation(nextCheck, new string[] { "divorce" }, allLocations))
                    {

                    }
                    else
                    {
                        batteryLocation = allLocations[nextCheck];
                        insertItem("item_deka_denchi", nextCheck);
                        spoilerLog.Add("Giga-Battery", allLocations[nextCheck].Description);
                        occupiedChecks[nextCheck] = true;
                        break;
                    }
                }
                else 
                {
                    if (occupiedChecks[nextCheck] == true || !validLocation(nextCheck, new string[] { "ladder", "bridge", "divorce" }, allLocations))
                    {

                    }
                    else
                    {
                        batteryLocation = allLocations[nextCheck];
                        insertItem("item_deka_denchi", nextCheck);
                        spoilerLog.Add("Giga-Battery", allLocations[nextCheck].Description);
                        occupiedChecks[nextCheck] = true;
                        break;
                    }
                }
                      
            }

            //Shuffle Toothbrush
            //Shuffling logic for this object will need to be tweaked as options for skipping sophie / randoing given objs are supported
            if (openDownstairs.Checked)
            {
                spoilerLog.Add("Toothbrush", allLocations[shuffleItem("item_brush", occupiedChecks, new string[] { "toothbrush" }, allLocations)].Description);
            }
            else
            {
                while (true)
                {
                    int nextCheck = r.Next(0, allLocations.Count() - 1);

                    if (occupiedChecks[nextCheck] == true || nextCheck > stageData.rooms[0].locations.Count || !validLocation(nextCheck, new string[] { "ladder", "bridge", "toothbrush" }, allLocations))
                    {

                    }
                    else
                    {
                        occupiedChecks[nextCheck] = true;
                        insertItem("item_brush", nextCheck);
                        spoilerLog.Add("Toothbrush", allLocations[nextCheck].Description);
                        break;
                    }
                }
            }            

            if (batteryLocation.Prereqs.Contains("squirter") || batteryLocation.Prereqs.Contains("frog suit") || chargerLocation.Prereqs.Contains("squirter") || chargerLocation.Prereqs.Contains("frog suit"))
            {
                spoilerLog.Add("Squirter", allLocations[shuffleItem("item_tyuusyaki", occupiedChecks, new string[] { "squirter", "frog suit", "ladder", "bridge" }, allLocations)].Description);
            }
            else 
            {
                spoilerLog.Add("Squirter", allLocations[shuffleItem("item_tyuusyaki", occupiedChecks, new string[] { "squirter", "frog suit" }, allLocations)].Description);
            
            }

            if (batteryLocation.Prereqs.Contains("blaster") || chargerLocation.Prereqs.Contains("blaster"))
            {
                spoilerLog.Add("Chibi-Blaster", allLocations[shuffleItem("cb_cannon_lv_2", occupiedChecks, new string[] { "ladder", "bridge", "blaster" }, allLocations)].Description);
            }
            else 
            {
                spoilerLog.Add("Chibi-Blaster", allLocations[shuffleItem("cb_cannon_lv_2", occupiedChecks, new string[] { "blaster" }, allLocations)].Description);
            }

            if (batteryLocation.Prereqs.Contains("radar") || chargerLocation.Prereqs.Contains("radar"))
            {
                spoilerLog.Add("Chibi-Radar", allLocations[shuffleItem("cb_radar", occupiedChecks, new string[] { "ladder", "bridge", "radar" }, allLocations)].Description);
            }
            else
            {
                spoilerLog.Add("Chibi-Radar", allLocations[shuffleItem("cb_radar", occupiedChecks, new string[] { "radar" }, allLocations)].Description);
            }


            if (batteryLocation.Prereqs.Contains("mug") || chargerLocation.Prereqs.Contains("mug"))
            {
                spoilerLog.Add("Mug", allLocations[shuffleItem("item_mag_cup", occupiedChecks, new string[] {"ladder", "bridge", "mug" }, allLocations)].Description);
            }
            else 
            {
                spoilerLog.Add("Mug", allLocations[shuffleItem("item_mag_cup", occupiedChecks, new string[] { "mug" }, allLocations)].Description);
            }

            if (batteryLocation.Prereqs.Contains("spoon") || chargerLocation.Prereqs.Contains("spoon"))
            {
                spoilerLog.Add("Spoon", allLocations[shuffleItem("item_spoon", occupiedChecks, new string[] { "spoon", "ladder", "bridge" }, allLocations)].Description);
            }
            else 
            {
                spoilerLog.Add("Spoon", allLocations[shuffleItem("item_spoon", occupiedChecks, new string[] { "spoon" }, allLocations)].Description);
            }

            if (batteryLocation.Prereqs.Contains("charge chip") || chargerLocation.Prereqs.Contains("charge chip"))
            {
                spoilerLog.Add("Charge Chip", allLocations[shuffleItem("item_chip_53", occupiedChecks, new string[] { "charge chip", "ladder", "bridge" }, allLocations)].Description);
            }
            else 
            {
                spoilerLog.Add("Charge Chip", allLocations[shuffleItem("item_chip_53", occupiedChecks, new string[] { "charge chip" }, allLocations)].Description);
            }

            spoilerLog.Add("Red Shoe", allLocations[shuffleItem("item_peets_kutu", occupiedChecks, new string[] { "red shoe", "ladder", "bridge" }, allLocations)].Description);
          

            if (batteryCharge.Checked) 
            {
                spoilerLog.Add("Charged Giga-Battery", allLocations[shuffleItem("item_deka_denchi_full", occupiedChecks, new string[] { }, allLocations)].Description);
            }

            #endregion

            //Checks that are Key Items but don't lock progression or check access
            #region Key Item (Misc.) Checks

            spoilerLog.Add("Toy Receipt", allLocations[shuffleItem("item_receipt", occupiedChecks, new string[] { "divorce" }, allLocations)].Description);

            spoilerLog.Add("Range Chip", allLocations[shuffleItem("item_chip_54", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Alien Ear Chip", allLocations[shuffleItem("item_hocyouki", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Chibi-Battery", allLocations[shuffleItem("item_c_denchi", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Free Rangers Photo", allLocations[shuffleItem("item_army_photo", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Red Block", allLocations[shuffleItem("item_t_block_6", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Green Block", allLocations[shuffleItem("item_t_block_4", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("White Block", allLocations[shuffleItem("item_t_block_3", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Red Crayon", allLocations[shuffleItem("item_kure_1", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Yellow Crayon", allLocations[shuffleItem("item_kure_3", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Green Crayon", allLocations[shuffleItem("item_kure_4", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Purple Crayon", allLocations[shuffleItem("item_kure_5", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Dog Tags", allLocations[shuffleItem("item_tug", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Bandage", allLocations[shuffleItem("item_houtai", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Ticket Stub", allLocations[shuffleItem("item_ticket", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Gunpowder", allLocations[shuffleItem("item_kayaku", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Hot Rod", allLocations[shuffleItem("item_car_item", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Space Scrambler", allLocations[shuffleItem("item_nwing_item", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Scurvy Splinter", allLocations[shuffleItem("npc_hock_ship_114", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Passed-Out Frog", allLocations[shuffleItem("item_frog", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Dinahs Teeth", allLocations[shuffleItem("item_rex_tooth", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Snorkel", allLocations[shuffleItem("item_goggle", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("AA Battery", allLocations[shuffleItem("item_denchi_3", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("C Battery", allLocations[shuffleItem("item_denchi_2", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("D Battery", allLocations[shuffleItem("item_denchi_1", occupiedChecks, new string[] { }, allLocations)].Description);

            spoilerLog.Add("Wedding Ring", allLocations[shuffleItem("item_papa_yubiwa", occupiedChecks, new string[] { }, allLocations)].Description);

            // 10 Frog Rings

            for (int i = 0; i < 10; i++) 
            {
                spoilerLog.Add("Frog Ring " + (i+1), allLocations[shuffleItem("item_frog_ring", occupiedChecks, new string[] { "shop" }, allLocations)].Description);
            }
            #endregion

            //Shuffle Leg
            while (true)
            {
                int nextCheck = r.Next(0, allLocations.Count() - 1);

                if (occupiedChecks[nextCheck] == true || !validLocation(nextCheck, new string[] { }, allLocations))
                {

                }
                else
                {
                    legLocation = allLocations[nextCheck];
                    occupiedChecks[nextCheck] = true;
                    insertItem("item_left_foot", nextCheck);
                    spoilerLog.Add("Giga-Robo's Left Leg", allLocations[nextCheck].Description);
                    break;
                }
            }

            stopwatch.Stop();
            #endregion

            #region New Implementation
            //Things to keep track of:
            //locked prereqs
            //what we have already shuffled
            //key items per room (rooms represented by ints thru stageData.rooms)

            List<string> lockedPrereqs = new List<string>();
            Dictionary<string, bool> shuffledKeyItems = new Dictionary<string, bool>();

            #region Data Init
            //NOTE: ladder / bridge removed if ( (charger && battery) && (blaster || toothbrush) )
            lockedPrereqs.AddRange(new string[] { "ladder", "bridge", "blaster", "mug", "squirter", "divorce", "spoon", "radar", "red shoe", "charge chip" });

            shuffledKeyItems.Add("item_brush", false);
            shuffledKeyItems.Add("item_tyuusyaki", false);
            shuffledKeyItems.Add("item_spoon", false);
            shuffledKeyItems.Add("item_mag_cup", false);
            shuffledKeyItems.Add("item_receipt", false);
            shuffledKeyItems.Add("item_chip_53", false);
            shuffledKeyItems.Add("item_chibi_house_denti_2", false);
            shuffledKeyItems.Add("item_deka_denchi", false);
            shuffledKeyItems.Add("item_left_foot", false);
            shuffledKeyItems.Add("item_peets_kutu", );
            shuffledKeyItems.Add("cb_radar", false);
            shuffledKeyItems.Add("cb_cannon_lv_2", false);
            shuffledKeyItems.Add("item_papa_yubiwa", false);

            #endregion 

            

            //Pick a random key item to shuffle first

            //Shuffle it somewhere logical
            ///Reroll chance increased by num key items in that room already
            ///Reroll chance decreased by num prereqs for the check(?)

            //Update shuffledKeyItems
            //Update lockedPrereqs depending on what got shuffled


            #endregion

            statusDialog.Text += "\nShuffling completed in " + stopwatch.Elapsed + ".";
            return spoilerLog;
        }


        //Randomizes the location of the given item
        private int shuffleItem(string objectName, List<bool> occupiedLocations, string[] prerequisites, List<ItemLocation> allChecks ) 
        {
            while (true)
            {
                int nextCheck = r.Next(0, allChecks.Count() - 1);

                if (occupiedLocations[nextCheck] == true || !validLocation(nextCheck, prerequisites, allChecks))
                {

                }
                else
                {
                    occupiedLocations[nextCheck] = true;
                    insertItem(objectName, nextCheck);                    
                    return nextCheck;
                }
            }
            
        }

        //Determines if a location is a valid position for an object given the prerequisites
        private bool validLocation(int location, string[] prerequisites, List<ItemLocation> allChecks)
        {
            foreach (string p in prerequisites) 
            {
                if (allChecks[location].Prereqs.Contains(p) || allChecks[location].Prereqs.Contains("suitcase"))
                    return false;
            }
            return true;
        }


        //Inserts objectName at given location, assuming location is pulled from allLocations
        private void insertItem(string objectName, int location) 
        {
            JToken token;

            //Index of the item location relative to the room that it is in 
            int relativeLocation = 0;

            //Living Room
            if (location < stageData.rooms[0].locations.Count())
            {
                //Getting the object from the exported living room and changing the name
                token = livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags[0]").AddAfterSelf("flash");
                        livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags[0]").AddAfterSelf("cull");
                        livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags[0]").AddAfterSelf("lift");
                        livingRoomObj.SelectToken("objects[" + stageData.rooms[0].locations[location].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }
            //Kitchen
            else if (location < stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count())
            {
                relativeLocation = location - stageData.rooms[0].locations.Count();

                //Getting the object from the exported kitchen and changing the name
                token = kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        kitchenObj.SelectToken("objects[" + stageData.rooms[1].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }

            //Drain
            else if (location < stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count())
            {
                relativeLocation = location - (stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());

                //Getting the object from the exported drain and changing the name
                token = drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        drainObj.SelectToken("objects[" + stageData.rooms[2].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }

            //Foyer
            else if (location < stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count())
            {
                relativeLocation = location - (stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());

                //Getting the object from the exported foyer and changing the name
                token = foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        foyerObj.SelectToken("objects[" + stageData.rooms[3].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }

            //Basement
            else if (location < stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count())
            {
                relativeLocation = location - (stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());

                //Getting the object from the exported basement and changing the name
                token = basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        basementObj.SelectToken("objects[" + stageData.rooms[4].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }

            //Backyard
            else if (location < stageData.rooms[5].locations.Count() + stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count())
            {
                relativeLocation = location - (stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());

                //Getting the object from the exported basement and changing the name
                token = backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        backyardObj.SelectToken("objects[" + stageData.rooms[5].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }

            //Jenny's Room
            else if (location < stageData.rooms[6].locations.Count() + stageData.rooms[5].locations.Count() + stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count())
            {
                relativeLocation = location - (stageData.rooms[5].locations.Count() + stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());

                //Getting the object from the exported basement and changing the name
                token = jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        jennyRoomObj.SelectToken("objects[" + stageData.rooms[6].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }
            
            //Bedroom
            else if (location < stageData.rooms[7].locations.Count() + stageData.rooms[6].locations.Count() + stageData.rooms[5].locations.Count() + stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count()) 
            {
                relativeLocation = location - (stageData.rooms[6].locations.Count() + stageData.rooms[5].locations.Count() + stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());

                //Getting the object from the exported basement and changing the name
                token = bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].object");
                token.Replace(objectName);

                //Setting the correct flags for the new object
                int finalFlagIndex = bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags").Children().Count() - 1;

                List<JToken> oldFlags = new List<JToken>();

                foreach (JToken flag in bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags").Children())
                {
                    oldFlags.Add(flag);
                }

                for (int i = 1; i < oldFlags.Count; i++)
                {
                    oldFlags[i].Remove();
                }

                switch (objectName)
                {
                    case "coin_c":
                    case "coin_s":
                    case "coin_g":
                    case "item_junk_a":
                    case "item_junk_b":
                    case "item_junk_c":
                        bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                    default:
                        bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("flash");
                        bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("cull");
                        bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("lift");
                        bedroomObj.SelectToken("objects[" + stageData.rooms[7].locations[relativeLocation].ID + "].flags[0]").AddAfterSelf("interact");
                        break;
                }
                return;
            }
            
            //Shop (aka hell)
            else
            {
                relativeLocation = location - (stageData.rooms[7].locations.Count() + stageData.rooms[6].locations.Count() + stageData.rooms[5].locations.Count() + stageData.rooms[4].locations.Count() + stageData.rooms[3].locations.Count() + stageData.rooms[2].locations.Count() + stageData.rooms[1].locations.Count() + stageData.rooms[0].locations.Count());
                token = shopObj.SelectToken("items[" + stageData.rooms[8].locations[relativeLocation].ID + "].item");

                //Shop checks take an item name rather than an object name. Using the supplied object name, we can get the item name of the matching object in itemPool.json
                string itemName = "";
                foreach (Item i in itemPool.Items) 
                {
                    if (i.objectName == objectName) 
                    {
                        itemName = i.itemName;
                    }
                }
                token.Replace(itemName);
            }
            
            //Cut this return statement out later~!
            return;
        }

        //Reimports the JSON stage and shop data into the ISO
        private void reimportStages() 
        {
            File.WriteAllText("stage01.json", kitchenObj.ToString());
            File.WriteAllText("stage02.json", foyerObj.ToString());
            File.WriteAllText("stage03.json", basementObj.ToString());
            File.WriteAllText("stage04.json", jennyRoomObj.ToString());
            File.WriteAllText("stage06.json", bedroomObj.ToString());
            File.WriteAllText("stage07.json", livingRoomObj.ToString());
            File.WriteAllText("stage09.json", backyardObj.ToString());
            File.WriteAllText("stage11.json", drainObj.ToString());
            File.WriteAllText("globals.json", globals.ToString());

            string test = shopObj.ToString().Substring(14, shopObj.ToString().Length - 15);
            //JSON formatting for the shop is borked so this is the reconversion into the form that Unplug is looking for
            File.WriteAllText("shop.json", shopObj.ToString().Substring(14, shopObj.ToString().Length - 15));

            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage01 \"" + Directory.GetCurrentDirectory() + @"\stage01.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage02 \"" + Directory.GetCurrentDirectory() + @"\stage02.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage03 \"" + Directory.GetCurrentDirectory() + @"\stage03.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage04 \"" + Directory.GetCurrentDirectory() + @"\stage04.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage06 \"" + Directory.GetCurrentDirectory() + @"\stage06.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage07 \"" + Directory.GetCurrentDirectory() + @"\stage07.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage09 \"" + Directory.GetCurrentDirectory() + @"\stage09.json" + "\"");
            runUnplugCommand("stage import --iso \"" + newIsoPath + "\" stage11 \"" + Directory.GetCurrentDirectory() + @"\stage11.json" + "\"");
            runUnplugCommand("globals import --iso \"" + newIsoPath + "\" \"" + Directory.GetCurrentDirectory() + @"\globals.json" + "\"");
            runUnplugCommand("shop import --iso \"" + newIsoPath + "\" \"" + Directory.GetCurrentDirectory() + @"\shop.json" + "\"");

            List<string> oldFiles = new List<string>();
            foreach (string f in Directory.GetFiles(Directory.GetCurrentDirectory()))
            {
                if (f.Substring(0, 4) == "stage")
                {
                    oldFiles.Add(f);
                }
            }
            foreach (string f in oldFiles)
            {
                File.Delete(f);
            }
        }

    }
}
