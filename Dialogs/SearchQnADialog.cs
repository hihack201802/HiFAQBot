using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using HiFaqBot01.Services;
using HiFaqBot01.Models;

namespace HiFaqBot01.Dialogs
{
    [Serializable]
    public class SearchQnADialog : IDialog<QnAResult>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(SearchQnAAsync);
        }
        private async Task SearchQnAAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var searchText = activity.Text;

            QnAMakerSearchService searchService = new QnAMakerSearchService();
            QnAResult qnaResult = await searchService.Search(searchText);

            context.Done(qnaResult);
        }
    }

}