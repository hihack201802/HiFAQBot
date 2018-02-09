using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using HiFaqBot01.Services;
using HiFaqBot01.Models;

namespace HiFaqBot01.Dialogs
{
    [Serializable]
    class SearchFaqDbDialog : IDialog<SearchResult>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(SearchFaqAsync);
        }

        private async Task SearchFaqAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var searchText = activity.Text;

            // 空白文字は OR検索に変更
            if (searchText.Contains(" ") || searchText.Contains("　"))
            {
                searchText = activity.Text.Replace(" ", "|").Replace("　", "|");
            }

            // 全文検索 (search=searchText)
            AzureSearchService searchService = new AzureSearchService();
            SearchResult searchResult = await searchService.Search(searchText);

            context.Done(searchResult);

            // カテゴリー内FAQ検索 (facet=Category1)
            //FacetResult facetResult = await searchService.FetchFacets();
            //if (facetResult.Facets.Category1.Length > 0)
            //{
            //    await context.PostAsync($"Category1 にある FAQ の数は" +
            //        facetResult.Facets.Category1.Length.ToString() + "個です");
            //}


        }

        private async Task SearchFaqAsync(IDialogContext context, IAwaitable<KeyphraseResult> result)
        {
            var searchText = "";
            var keyphraseResult = await result as KeyphraseResult;

            // Keyphrase をOR検索として整形
            if (keyphraseResult.Documents[0].KeyPhrases != null)
            {
                foreach (var keyphrase in keyphraseResult.Documents[0].KeyPhrases)
                {
                    searchText = keyphrase + "|" + searchText;
                }
            }

            // 全文検索 (search=searchText)
            AzureSearchService searchService = new AzureSearchService();
            SearchResult searchResult = await searchService.Search(searchText);

            context.Done(searchResult);

        }


    }
}