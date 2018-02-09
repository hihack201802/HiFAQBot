namespace HiFaqBot01.Models
{
    using Newtonsoft.Json;

    public class SearchFacets
    {
        [JsonProperty("category@odata.type")]
        public string CategoryOdataType { get; set; }

        public Category[] Category1 { get; set; }
        public Category[] Category2 { get; set; }
        public Category[] Category3 { get; set; }
    }
}