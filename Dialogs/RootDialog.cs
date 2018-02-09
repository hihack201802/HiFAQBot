using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using HiFaqBot01.Models;
using Microsoft.Cognitive.LUIS;
using System.Linq;
using System.Collections.Generic;
using System.Configuration;


namespace HiFaqBot01.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            // メインメニュー
            context.Wait(MessageReceivedAsync);
            // はじめの入力からLUISを使う場合
            //context.Wait(LuisMessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // 初期メニュー
            var reply = activity.CreateReply("Welcome! FAQBot です。\n" +
                        "何かお困りですか？ 検索したい内容を文章で入れてください。\n" +
                        "または以下のメニューからお選びください。");

            reply.Type = ActivityTypes.Message;
            reply.TextFormat = TextFormatTypes.Plain;
            reply.SuggestedActions = new SuggestedActions()
            {
                Actions = new List<CardAction>()
                {
                    new CardAction(){ Title = "よくあるご質問", Type=ActionTypes.ImBack, Value="よくあるご質問"},
                    new CardAction(){ Title = "FAQ検索",Type=ActionTypes.ImBack, Value="FAQ検索"},
                    new CardAction(){ Title = "FAQリスト表示", Type=ActionTypes.ImBack, Value="FAQリスト表示"},
                    new CardAction(){ Title = "ヘルプ", Type=ActionTypes.ImBack, Value="ヘルプ"}
                }
            };

            await context.PostAsync(reply);
            context.Wait(MenuResumeAfter);

        }

        private async Task LuisMessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = result as Activity;

            //var luisClient = new LuisClient("b8ef2016-25ca-4278-8f76-1c79561c433f", "299ab024237441e499dd999c8cb2b825", domain: "westus");
            var luisClient = new LuisClient(ConfigurationManager.AppSettings["LuisAppId"], ConfigurationManager.AppSettings["LuisSubKey"], domain: "westus");

            
            
            var luisResult = await luisClient.Predict(activity.Text);

            switch (luisResult.Intents[0].Name)
            {
                case "Help":
                    // ヘルプメニュー表示
                    await context.PostAsync($"【ヘルプ】(score: " + luisResult.Intents[0].Score + ") \n\n" +
                        "何かお困りですか？ 検索したい内容を文章 またはキーワードで入れてください。\n\n" +
                        // DB を直接 AzureSearch で検索する場合
                        "複数の単語を空白で区切って入力すると、キーワード検索になります。"
                        );
                    context.Wait(MessageReceivedAsync);
                    break;

                case "SearchQA":
                    // FAQ検索
                    await context.PostAsync($"【FAQ検索】(score: " + luisResult.Intents[0].Score + ")");
                    // AzureSearch で検索
                    //await context.Forward(new SearchFaqDbDialog(), SearchFaqDBResumeAfter, activity);
                    // QnAMaker で検索
                    await context.Forward(new SearchQnADialog(), SearchQnAResumeAfter, activity);
                    break;

                case "ListFAQ":
                    //FAQリスト表示
                    await context.PostAsync($"【FAQリスト表示 - 未実装】(score: " + luisResult.Intents[0].Score + ")");  //未実装
                    context.Wait(MessageReceivedAsync);
                    break;

                default: // case "Greeting", "None"
                    // 初期メッセージ表示
                    await context.PostAsync($"【初期メニュー】(score: " + luisResult.Intents[0].Score + ") \n\n" +
                        "こんにちは。FAQ Bot です。\n\n" +
                        "何かお困りですか？ 検索したい内容を文章 またはキーワードで入れてください。");
                    context.Wait(MessageReceivedAsync);
                    break;

            }

        }

        private async Task ExtractKeyword(IDialogContext context,IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;
            await context.Forward(new KeywordExtractDialog(), ExtractKeywordResumeAfter, activity);
        }

        private async Task ExtractKeywordResumeAfter(IDialogContext context, IAwaitable<KeyphraseResult> result)
        {
            var keyphraseResult = await result as KeyphraseResult;
            var keyword = new Keyword();
            if (keyphraseResult.Documents[0].KeyPhrases != null)
            {
                foreach(var document in keyphraseResult.Documents)
                {
                    keyword.Value = document.KeyPhrases.ToString();
                }
            }
            context.Done(keyword);
        }

        private async Task MenuResumeAfter(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var selectedMenu = await result;
            var activity = await result as Activity;
            switch (selectedMenu.Text)
            {
                case "よくあるご質問":
                    await context.PostAsync($"【よくあるご質問表示 - 未実装】");
                    context.Done(this);
                    break;
                case "FAQ検索":
                    await context.PostAsync($"【FAQ検索】検索したい内容を文章で入れてください。");
                    context.Wait(SearchFaq);
                    break;
                case "FAQリスト表示":
                    await context.PostAsync($"【FAQリスト表示 - 未実装】");
                    context.Done(this);
                    break;
                case "ヘルプ":
                    await context.PostAsync($"【ヘルプ - 未実装】");
                    context.Done(this);
                    break;
                default:
                    await context.PostAsync($"FAQから検索します。");
                    await SearchFaq(context,result);
                    break;
            }
        }

        private async Task SearchFaq(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var activity = await result as Activity;
            // FAQ検索
            // AzureSearch で検索
            //await context.Forward(new SearchFaqDbDialog(), SearchFaqDBResumeAfter, activity);
            // QnAMaker で検索
            await context.Forward(new SearchQnADialog(), SearchQnAResumeAfter, activity);
        }

        private async Task SearchFaqDBResumeAfter(IDialogContext context, IAwaitable<SearchResult> result)
        {
            var searchResult = await result as SearchResult;
            if (searchResult.Value.Length != 0 && searchResult.Value[0].SearchScore > 0.5)
            {
                await context.PostAsync($"以下の FAQ が見つかりました。\n\n" +
                "【Q】" + searchResult.Value[0].Question + "\n\n" +
                "【A】" + searchResult.Value[0].Answer + "\n\n" +
                "(score: " + searchResult.Value[0].SearchScore + ")");

            }
            else
            {
                await context.PostAsync($"もう少し検索語を増やして検索してみてください。\n\n" +
                "(max.score: " + searchResult.Value[0].SearchScore + ")");
            }

            context.Done(this);

        }

        private async Task SearchQnAResumeAfter(IDialogContext context, IAwaitable<QnAResult> result)
        {
            //var qnaResults = await result as QnAMakerResults;
            var qnaResult = await result as QnAResult;

            if (qnaResult.Answers.Count() > 0)
            {
                await context.PostAsync($"以下の FAQ が見つかりました。\n\n" +
                "【Q】" + qnaResult.Answers[0].Questions[0] + "\n\n" +
                "【A】" + qnaResult.Answers[0].Answer + "\n\n" +
                "(score: " + qnaResult.Answers[0].Score + ")");

            }
            else
            {
                await context.PostAsync($"もう少し言葉を変えて検索してみてください。\n\n" +
                "(max.score: " + qnaResult.Answers[0].Score + ")");
            }

            context.Done(this);

        }
    }
}