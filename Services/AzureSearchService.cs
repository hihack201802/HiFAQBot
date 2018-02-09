namespace HiFaqBot01.Services
{
    using HiFaqBot01.Models;
    using Newtonsoft.Json;
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Configuration;

    [Serializable]
    public class AzureSearchService
    {
        private readonly string QueryString = $"https://{ConfigurationManager.AppSettings["AzureSearchAccount"]}.search.windows.net/indexes/{ConfigurationManager.AppSettings["AzureSearchIndex"]}/docs?api-key={ConfigurationManager.AppSettings["AzureSearchKey"]}&api-version=2016-09-01&";
        //private readonly string QueryString = $"https://hifaqbotsample.search.windows.net/indexes/faqlistdb31-index/docs?api-key=3EB906FACC978F131A710310FC4CDB73&api-version=2016-09-01&";


        public async Task<SearchResult> SearchByCategory1(string category)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{QueryString}$filter=Category1 eq '{category}'";
                string response = await httpClient.GetStringAsync(nameQuery);
                return JsonConvert.DeserializeObject<SearchResult>(response);
            }
        }
        public async Task<SearchResult> SearchByCategory2(string category)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{QueryString}$filter=Category2 eq '{category}'";
                string response = await httpClient.GetStringAsync(nameQuery);
                return JsonConvert.DeserializeObject<SearchResult>(response);
            }
        }

        public async Task<FacetResult> FetchFacets()
        {
            using (var httpClient = new HttpClient())
            {
                string facetQuery = $"{QueryString}facet=Category1";
                string response = await httpClient.GetStringAsync(facetQuery);
                return JsonConvert.DeserializeObject<FacetResult>(response);
            }
        }

        public async Task<SearchResult> Search(string text)
        {
            using (var httpClient = new HttpClient())
            {
                string nameQuery = $"{QueryString}search={text}";
                string response = await httpClient.GetStringAsync(nameQuery);
                return JsonConvert.DeserializeObject<SearchResult>(response);
            }
        }


    }
}