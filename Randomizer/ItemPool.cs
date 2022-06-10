using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WindowsFormsApp1
{
    public class ItemPool
    {
        [JsonProperty("items")]
        public List<Item> Items { get; set; }
    }
    public class Item 
    {
        public string name { get; set; }
        public int numChecks { get; set; }
        public string[] flags { get; set; }
    }

    //Handles check locations
    public class RootObject 
    {
        [JsonProperty("rooms")]
        public List<StageObject> rooms { get; set; }
    }

    public class StageObject 
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("checks")]
        public List<ItemLocation> locations { get; set; }
    }

    public class ItemLocation 
    {
        //Getters & Setters
        [JsonProperty("x")]
        public float X { get; set; }

        [JsonProperty("y")]
        public float Y { get; set; }

        [JsonProperty("z")]
        public float Z { get; set; }

        [JsonProperty("id")]
        public int ID { get; set; }

        [JsonProperty("name")]
        public string ObjectName { get; set; }

        [JsonProperty("prereqs")]
        public string[] Prereqs { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

    }
}
