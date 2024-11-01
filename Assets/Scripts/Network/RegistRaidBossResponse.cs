using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RegistRaidBossResponse
{
    [JsonProperty("id")]

    public int Id { get; set; }

    [JsonProperty("name")]

    public string Name { get; set; }

    [JsonProperty("maxHp")]

    public int maxHp { get; set; }

    [JsonProperty("nowHp")]
    
    public int nowHp { get; set; }
}

