using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class ConferenceDetailsDialog : IDialog<object>
    {
        private int NumberOfConferencesToSkip = 0;
        private int NumerOfConferencesOnOneScreen = 3;
        private string SearchTerm;

        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }
        public virtual async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var activity = await argument as Activity;
            SearchTerm = string.Join("", activity.Text.Split(' ').Skip(1)).Trim();
            await DisplayConferenceInfo(context);
        }
        public virtual async Task DisplayConferenceInfo(IDialogContext context)
        {

            var conference = (await ConferenceData.Instance.GetConferencesAsync(SearchTerm)).ToList();

            if (conference == null)
            {
                await context.PostAsync("Can't find conferences containing: " + SearchTerm);
            }
            var replyToConversation = context.MakeMessage();
            replyToConversation.Text = "Conferences for term "+ SearchTerm;

            var allConferences = conference;
            var cards = conference
                            .Skip(NumberOfConferencesToSkip)
                            .Take(NumerOfConferencesOnOneScreen)
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

            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.List;
            replyToConversation.Attachments = new List<Attachment>(cards.ToList());

            await context.PostAsync(replyToConversation);

            NumberOfConferencesToSkip += NumerOfConferencesOnOneScreen;
            var numberOfConverencesThatLeft = allConferences.Count() - NumberOfConferencesToSkip;

            if (numberOfConverencesThatLeft > 0)
            {
                PromptDialog.Confirm(
                  context,
                  AfterResetAsync,
                  $"Would you like to see next {NumerOfConferencesOnOneScreen} of {numberOfConverencesThatLeft} ",
                  "Didn't get that!",
                  promptStyle: PromptStyle.None);
            }
            else
            {
                context.Done("Thank you from Conference Dialog.");
            }

        }




        private async Task AfterResetAsync(IDialogContext context, IAwaitable<bool> result)
        {
            var confirm = await result;
            if (confirm)
            {
                await context.PostAsync("Showing next conferences");
                await DisplayConferenceInfo(context);

            }
            else
            {
                context.Done("Thank you  frm Conference Dialog.");
            }
        }
    }
}