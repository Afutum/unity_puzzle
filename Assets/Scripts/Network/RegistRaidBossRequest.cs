using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;


public class RegistRaidBossRequest
{
    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("user_id")]

    public int UserId { get; set; }

    [JsonProperty("boss_id")]

    public int BossId { get; set; }

    [JsonProperty("damage")]

    public int Damage { get; set; }
}

