﻿using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Bot_Application1
{
    public class ConferenceData
    {
        private const string ConferenceUrl = "https://www.codeeurope.pl/api/calendar_event/all";

        Task<IList<Event>> Events;
        Task<List<SpeakerList>> Speakers;

        public static ConferenceData Instance = new ConferenceData();

        public ConferenceData()
        {
            GetEvents();
        }

        private async Task<IList<Event>> GetEvents()
        {
            if(Events == null)
            {
                using (WebClient wc = new WebClient())
                {
                    wc.Headers.Add(HttpRequestHeader.Accept, "application/vnd.absolvent.v1+json");
                    var conferenceData = wc.DownloadStringTaskAsync(ConferenceUrl);
                    Events = Task.FromResult(Event.FromJson(await conferenceData));

                }
            }
            return await Events;
        }
        private async Task<List<SpeakerList>> GetSpeakers()
        {
            if (Speakers == null)
            {
                var speakers = (await GetEvents())
                    .SelectMany(e => e.SpeakerList)
                    .GroupBy(x => x.SpeakerId)
                    .Where(c => c.Count() >= 1)
                    .Select(x => x.FirstOrDefault())
                    .ToList();
                Speakers = Task.FromResult(speakers);
            }
            return await Speakers;
        }
        public async Task<SpeakerList> GetSpeakerAsync(string name)
        {
            var normalizedName = (name??"").ToUpper();
            return (await GetSpeakers()).FirstOrDefault(e=>e.SpeakerName.ToUpper().Contains(normalizedName));
        }

    }
}