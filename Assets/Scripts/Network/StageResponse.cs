using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class StageResponse
{
    [JsonProperty("stage_id")]
    public int StageID { get; set; }

    [JsonProperty("clear_count")]
    public int StageClearCount { get; set; }

    [JsonProperty("fastest_time")]
    public float StageFastestTime { get; set; }
}