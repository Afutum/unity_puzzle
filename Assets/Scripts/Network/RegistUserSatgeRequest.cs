using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class RegistUserStageRequest
{
    [JsonProperty("user_id")]
    public int UserId { get; set; }

    [JsonProperty("stage_id")]
    public int StageID { get; set; }

    [JsonProperty("fastest_time")]
    public int FastestTime {  get; set; }
}
