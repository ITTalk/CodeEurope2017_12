using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.FormFlow;
using Microsoft.Bot.Connector;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class ConferenceSearchDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            var conferenceSearchParameters = FormDialog.FromForm(BuildForm,FormOptions.PromptInStart);
            context.Call(conferenceSearchParameters,ResumeAfterConferenceSearchAsync);
            return Task.CompletedTask; 
        }

        public static IForm<ConferenceSearch> BuildForm()
        {
            OnCompletionAsyncDelegate<ConferenceSearch> processHotelsSearch = async (ctx, state) =>
            {
                await ctx.PostAsync($"Ok. Searching for Conferences in {state.Language} in {state.City}. Tracks {string.Join(",",state.Tracks.Select(t=>t.ToString()))} ...");
            };
            return new FormBuilder<ConferenceSearch>()
                    .Message("Welcome to conference search!")
                    .OnCompletion(processHotelsSearch)
                    .Build();
        }

        private async Task ResumeAfterConferenceSearchAsync(IDialogContext context, IAwaitable<ConferenceSearch> result)
        {
            var searchCriteria = await result;
            var allConferences = (await ConferenceData.Instance.GetConferencesAsync()).ToList();
           var conferences = allConferences.Where(c =>
            c.CityName == searchCriteria.City.ToString() &&
            (!searchCriteria.Language.HasValue || c.EventLanguageName == searchCriteria.Language.ToString()) &&
            (!searchCriteria.Types.Any() || searchCriteria.Types.ToString().ToLower().Contains(c.EventType)) &&
            (!searchCriteria.Tracks.Any() || searchCriteria.Tracks.Select(t => t.ToString().ToSnakeCase()).Any(cc => c.EventTrack.Contains(cc)))
            ).ToList();

            if (!conferences.Any())
            {
                await context.PostAsync("Sorry, we couldn't find anything matching your criteria");
                context.Done("");
                return;
            }

            var cards = conferences
                            .Select(conf =>
                            {

                                var eventTime = DateTimeOffset.FromUnixTimeMilliseconds(conf.EventStartTimestamp);

                                var plCard = new ThumbnailCard()
                                {
                                    Title = $"{conf.EventName}, ({ Enumerable.Repeat("*", (int)conf.DifficultyRating) })",
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

            context.Done("");
        }
    }
}

public static class ExtensionMethods
{
    public static string ToSnakeCase(this string str)
    {
        return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToLower();
    }
}