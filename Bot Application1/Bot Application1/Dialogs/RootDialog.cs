using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;
using System.Threading;
using Bot_Application1.Cards;

namespace Bot_Application1.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            if (activity.Text.Contains("speaker") || activity.Text.Contains("whois"))
            {
               await context.PostAsync(await new UserInfoCard().GetUserInfoCardAsync(activity));
               context.Wait(MessageReceivedAsync);
            }
            else if (activity.Text.Contains("conf"))
            {
                await context.Forward(new ConferenceDetailsDialog(), NameDialogResumeAfter,activity,CancellationToken.None);
                return;
            }

            await context.PostAsync("I didn't understand. Try spekaer .... or conf ....");
            context.Wait(MessageReceivedAsync);
        }



        private async Task NameDialogResumeAfter(IDialogContext context, IAwaitable<object> result)
        {
            context.Wait(MessageReceivedAsync);
        }




    }
}