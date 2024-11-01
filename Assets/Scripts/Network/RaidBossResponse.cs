using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class RaidBossResponse
{
    [JsonProperty("id")]

    public int Id { get; set; }

    [JsonProperty("boss_id")]

    public int BossId { get; set; }

    [JsonProperty("nowHp")]

    public int NowHp {  get; set; }
}

