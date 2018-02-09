﻿namespace HiFaqBot01.Models
{
    using Newtonsoft.Json;

    public class SearchResult
    {
        [JsonProperty("@odata.context")]
        public string ODataContext { get; set; }

        public SearchResultHit[] Value { get; set; }
    }
}