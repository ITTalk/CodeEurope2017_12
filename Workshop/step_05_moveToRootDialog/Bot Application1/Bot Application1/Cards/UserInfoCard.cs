﻿using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1.Cards
{
    public class UserInfoCard
    {
        public async Task<Activity> GetUserInfoCardAsync(Activity activity)
        {
            var userName = string.Join("",activity.Text.Split(' ').Skip(1)).Trim();

            var speaker = await ConferenceData.Instance.GetSpeakerAsync(userName);
            if(speaker == null)
            {
                return activity.CreateReply($"Can't find any user matching ${userName}");
            }

            Activity replyToConversation = activity.CreateReply();
            replyToConversation.AttachmentLayout = AttachmentLayoutTypes.Carousel;

            var cardImages = new List<CardImage>
                {
                    new CardImage(speaker.SpeakerImage )
                };

            var plCard = new ThumbnailCard()
            {
                Title = speaker.SpeakerName,
                Subtitle = speaker.SpeakerTitle,
                Text = speaker.SpeakerDescription,
                Images = cardImages,
                Tap = new CardAction()
                {
                    Type="openUrl",
                    Title="Open User Page",
                    Value = $"https://www.codeeurope.pl/{speaker.SpeakerUrl.En}"
                }
            };

            replyToConversation.Attachments = new List<Attachment>(1) { plCard.ToAttachment() };
            return replyToConversation;
        }
    }
}