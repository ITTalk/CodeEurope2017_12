using Microsoft.Bot.Builder.FormFlow;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Bot_Application1.Dialogs
{
    public enum ConferenceLocationOptions
    {
        Warszawa, Kraków, Wrocław
    }
    public enum ConferencLanguageOptions
    {
        English, Polish
    }
    public enum ConferenceKindOptions
    {
        Workshop=1,Lecture
        
    }
    public enum ConferenceTrackOptions
    {
        FutureInspire=1,
        CloudComputing,
        DataScience,
        JavaScala,
        TomorrowWeb,
        DevopsArchitectureMicroservices,
        ProgrammingLanguages, 
        EverythingHardware,
        SecurityTesting,
        Gamede
    }

    [Serializable]
    public class ConferenceSearch
    {
        public ConferenceLocationOptions? City { get; set; }
        [Optional]
        public ConferencLanguageOptions? Language { get; set; }
        [Optional]
        public List<ConferenceKindOptions> Types { get; set; }
        [Prompt("Please select {&} from below list? {||}")]
        public List<ConferenceTrackOptions> Tracks { get; set; }
        [Optional]

        [Template(TemplateUsage.NoPreference,"no","No")]
        public string Title { get; set; }

    };
 
}