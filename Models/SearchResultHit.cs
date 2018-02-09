namespace HiFaqBot01.Models
{
    using Newtonsoft.Json;

    public class SearchResultHit
    {
        [JsonProperty("@search.score")]
        public float SearchScore { get; set; }

        public string Question { get; set; }

        public string Answer { get; set; }

        public string Category1 { get; set; }
        public string Category2 { get; set; }
        public string Category3 { get; set; }

        public string KeyPhrase1 { get; set; }
        public string KeyPhrase2 { get; set; }
        public string KeyPhrase3 { get; set; }
        public string KeyPhrase4 { get; set; }

        public int FAQInquiry { get; set; }


    }
}