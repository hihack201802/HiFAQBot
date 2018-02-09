namespace HiFaqBot01.Services
{
    using HiFaqBot01.Models;
    using Newtonsoft.Json;
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;
    using System.Configuration;

    [Serializable]
    public class TextAnalyticsKeyphraseService
    {
        //private readonly string textAnalyticsSubKey = "42e890da47a8480a850bc914d81c2b76";
        private readonly string textAnalyticsSubKey = ConfigurationManager.AppSettings["TextAnalyticsSubKey"];

        Uri qnaBaseUri = new Uri("https://eastasia.api.cognitive.microsoft.com/text/analytics/v2.0");

        public async Task<KeyphraseResult> Search(string text)
        {
            using (var httpClient = new HttpClient())
            {
                var qnaUri = $"{qnaBaseUri}/keyPhrases";
                var postBody = $"{{\"documents\": [{{\"id\": \"string\",\"{text}\": \"string\"}}]}}";

                var content = new StringContent(postBody, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", textAnalyticsSubKey);
                var postResult = await httpClient.PostAsync(qnaUri, content);
                var response = await postResult.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<KeyphraseResult>(response);
            }
        }


    }
}