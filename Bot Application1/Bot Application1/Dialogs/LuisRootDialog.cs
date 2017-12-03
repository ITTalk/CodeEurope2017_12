using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using System.Linq;
using System.Collections.Generic;
using Bot_Application1.Cards;
using Newtonsoft.Json.Linq;

namespace Bot_Application1.Dialogs
{
    [LuisModel("5c992c62-af70-41b4-909f-9f38e520e18a", "397f98a8a5e1472f8861f8f286fe3bbf", verbose:true)]
    [Serializable]
    public class LuisRootDialog : LuisDialog<object>
    {
    
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string message = $"Sorry, I did not understand '{result.Query}'. Type 'help' if you need assistance.";

            await context.PostAsync(message);

            context.Wait(this.MessageReceived);
        }

        [LuisIntent("SpeakerDetails")]
        public async Task SpeakerDetails(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {

            await context.PostAsync(await new UserInfoCard().GetUserInfoCardAsync(context, result));
            context.Wait(this.MessageReceived);
        }


        [LuisIntent("Conference Search")]
        public async Task Search(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            var message = await activity;
            await context.PostAsync($"Welcome to Conference finder! We are analyzing your message: '{message.Text}'...");

            var conferenceSearch = new ConferenceSearch();


            if (result.TryFindEntity("title", out EntityRecommendation titleEntity))
            {
                titleEntity.Type = "Title";
                
            }
            if (result.TryFindEntity("language", out EntityRecommendation language))
            {
                language.Type = "Language";
                SetRecomendationAsEntity(language);
             }
            if (result.TryFindEntity("city", out EntityRecommendation cities))
            {
                cities.Type = "City";
                SetRecomendationAsEntity(cities);
            }

            var confFormDialog = new FormDialog<ConferenceSearch>(conferenceSearch, BuildForm, FormOptions.PromptInStart, result.Entities);

            context.Call(confFormDialog, ResumeAfterConferenceSearchAsync);
        }
        private void SetRecomendationAsEntity(EntityRecommendation recomendation)
        {
            if (recomendation.Resolution != null)
            {
                recomendation.Entity = ((JArray)recomendation.Resolution.FirstOrDefault().Value).Values<string>().First();
                recomendation.Resolution = null;

            }

        }


        public IForm<ConferenceSearch> BuildForm()
        {
            OnCompletionAsyncDelegate<ConferenceSearch> processConferenceSearch = async (ctx, state) =>
            {
                var message = "Searching for conferences";
                if (state.Language.HasValue)
                {
                    message += $" in {state.Language}...";
                }
                if (state.City.HasValue)
                {
                    message += $" in city: {state.City}";
                }
                if (!string.IsNullOrWhiteSpace(state.Title))
                {
                    message += $" title:  {string.Join(",", state.Title)}...";
                }
                await ctx.PostAsync(message);
            };
            return new FormBuilder<ConferenceSearch>()
                    .Field(nameof(ConferenceSearch.Language),(state)=> !state.Language.HasValue)
                    .Field(nameof(ConferenceSearch.City), (state) => !state.City.HasValue)
                    .Field(nameof(ConferenceSearch.Title), (state) => string.IsNullOrWhiteSpace(state.Title))
                    .OnCompletion(processConferenceSearch)
                    .Build();
        }

        private async Task ResumeAfterConferenceSearchAsync(IDialogContext context, IAwaitable<ConferenceSearch> result)
        {
            var searchCriteria = await result;
            var allConferences = (await ConferenceData.Instance.GetConferencesAsync()).ToList();
            var conferences = allConferences.Where(c =>
             (!searchCriteria.City.HasValue || c.CityName == searchCriteria.City.ToString() )&&
             (string.IsNullOrWhiteSpace(searchCriteria.Title) || c.EventName.ToUpper().Contains(searchCriteria.Title.ToUpper())) &&
             (!searchCriteria.Language.HasValue || c.EventLanguageName == searchCriteria.Language.ToString()) 
             ).ToList();

            if (!conferences.Any())
            {
                await context.PostAsync("Sorry, we couldn't find anything matching your criteria");
                context.Done("");
                return;
            }

            var cards = conferences.Take(4)
                            .Select(conf =>
                            {

                                var eventTime = DateTimeOffset.FromUnixTimeMilliseconds(conf.EventStartTimestamp);
                                 var plCard = new ThumbnailCard()
                                {
                                    Title = $"{conf.EventName}, ({new string('*', (int)conf.DifficultyRating)})",
                                    Subtitle = $"{conf.CityName},, {eventTime.ToString("dddd dd MMMM")}",
                                    Text = conf.EventDescription,
                                    Tap = new CardAction()
                                    {
                                        Type = "openUrl",
                                        Title = "Add to calendar",
                                        Value = conf.EventMobileIcalUrl
                                    }
                                };
                                return plCard.ToAttachment();
                            });
            var replyToConversation = context.MakeMessage();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
            replyToConversation.Attachments = new List<Attachment>(cards.ToList());
            await context.PostAsync(replyToConversation);

            context.Done<object>(null);
        }


    }
}