using Newtonsoft.Json;

namespace The_Post.Models.API
{
    public class SpotNow
    {
        [JsonProperty("date")]
        public string Date { get; set; }
        public SERegion[] SE1 { get; set; }
        public SERegion[] SE2 { get; set; }
        public SERegion[] SE3 { get; set; }
        public SERegion[] SE4 { get; set; }
    }
}

public class SERegion
{
    [JsonProperty("hour")]
    public int Hour { get; set; }
    public float price_eur { get; set; }
    public float price_sek { get; set; }
    public int kmeans { get; set; }
}

//public class SE2
//{
//    [JsonProperty("hour")]
//    public int Hour { get; set; }
//    public float price_eur { get; set; }
//    public float price_sek { get; set; }
//    public int kmeans { get; set; }
//}

//public class SE3
//{
//    [JsonProperty("hour")]
//    public int Hour { get; set; }
//    public float price_eur { get; set; }
//    public float price_sek { get; set; }
//    public int kmeans { get; set; }
//}

//public class SE4
//{
//    [JsonProperty("hour")]
//    public int Hour { get; set; }
//    public float price_eur { get; set; }
//    public float price_sek { get; set; }
//    public int kmeans { get; set; }
//}
