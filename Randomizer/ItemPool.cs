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
        List<string> keyItemNames = new List<string>();

        public ItemPool(List<string> excludedItems) 
        {
            //Default 10 Frog Rings in the Pool
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");
            keyItemNames.Add("item_frog_ring");

            //Equipment
            keyItemNames.Add("");
        }
    }

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

    }
}
