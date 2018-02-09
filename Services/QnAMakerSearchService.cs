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
    public class QnAMakerSearchService
    {
        //private readonly string qnaKbId = "54e82d5d-f754-40df-bc20-ee8e29bcb6fa";
        //private readonly string qnaSubKey = "c400cb555d8f469db8bc77680a264a9e";
        private readonly string qnaKbId = ConfigurationManager.AppSettings["QnAMakerKnowledgebaseId"];
        private readonly string qnaSubKey = ConfigurationManager.AppSettings["QnAMakerSubscriptionKey"];
        Uri qnaBaseUri = new Uri("https://westus.api.cognitive.microsoft.com/qnamaker/v2.0");

        public async Task<QnAResult> Search(string text)
        {
            using (var httpClient = new HttpClient())
            {
                var qnaUri = $"{qnaBaseUri}/knowledgebases/{qnaKbId}/generateAnswer";
                //var postBody = $"{{\"question\": \"{text}\"}}";
                var postBody = $"{{\"question\": \"{text}\",\"top\": 3}}";

                var content = new StringContent(postBody, Encoding.UTF8, "application/json");
                httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", qnaSubKey);
                var postResult = await httpClient.PostAsync(qnaUri, content);
                var response = await postResult.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<QnAResult>(response);
            }
        }


    }
}