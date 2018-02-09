using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System;
using HiFaqBot01.Services;
using HiFaqBot01.Models;

namespace HiFaqBot01.Dialogs
{
    [Serializable]
    public class KeywordExtractDialog : IDialog<KeyphraseResult>
    {
        public async Task StartAsync(IDialogContext context)
        {
            context.Wait(KeyphraseExtractAsync);
        }
        private async Task KeyphraseExtractAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;
            var searchText = activity.Text;

            TextAnalyticsKeyphraseService keyphraseService = new TextAnalyticsKeyphraseService();
            KeyphraseResult keyphraseResult = await keyphraseService.Search(searchText);
            context.Done(keyphraseResult);
        }
    }

}