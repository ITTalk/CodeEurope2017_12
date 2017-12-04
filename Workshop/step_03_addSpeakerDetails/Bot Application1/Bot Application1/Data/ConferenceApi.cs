using Newtonsoft.Json;
using System.Collections.Generic;

namespace Bot_Application1
{
    //https://quicktype.io/?l=cs&r=json2csharp#l=cs&r=json2csharp
    public partial class Event
    {
        [JsonProperty("city_name")]
        public string CityName { get; set; }

        [JsonProperty("city_slug")]
        public string CitySlug { get; set; }

        [JsonProperty("difficulty_rating")]
        public long DifficultyRating { get; set; }

        [JsonProperty("difficulty_rating_text")]
        public string DifficultyRatingText { get; set; }

        [JsonProperty("event_address")]
        public string EventAddress { get; set; }

        [JsonProperty("event_attendant_advice")]
        public object EventAttendantAdvice { get; set; }

        [JsonProperty("event_description")]
        public string EventDescription { get; set; }

        [JsonProperty("event_desktop_ical_url")]
        public string EventDesktopIcalUrl { get; set; }

        [JsonProperty("event_end_timestamp")]
        public long EventEndTimestamp { get; set; }

        [JsonProperty("event_eventbrite_address")]
        public string EventEventbriteAddress { get; set; }

        [JsonProperty("event_id")]
        public long EventId { get; set; }

        [JsonProperty("event_is_custom")]
        public bool EventIsCustom { get; set; }

        [JsonProperty("event_is_enrollable")]
        public bool EventIsEnrollable { get; set; }

        [JsonProperty("event_is_lecture")]
        public bool EventIsLecture { get; set; }

        [JsonProperty("event_is_paid")]
        public long EventIsPaid { get; set; }

        [JsonProperty("event_is_workshop")]
        public bool EventIsWorkshop { get; set; }

        [JsonProperty("event_language_id")]
        public long EventLanguageId { get; set; }

        [JsonProperty("event_language_name")]
        public string EventLanguageName { get; set; }

        [JsonProperty("event_material_github_link")]
        public object EventMaterialGithubLink { get; set; }

        [JsonProperty("event_material_pdf_path")]
        public object EventMaterialPdfPath { get; set; }

        [JsonProperty("event_material_youtube_link")]
        public object EventMaterialYoutubeLink { get; set; }

        [JsonProperty("event_mobile_ical_url")]
        public string EventMobileIcalUrl { get; set; }

        [JsonProperty("event_name")]
        public string EventName { get; set; }

        [JsonProperty("event_participants_count")]
        public long EventParticipantsCount { get; set; }

        [JsonProperty("event_skills_obtainable")]
        public object EventSkillsObtainable { get; set; }

        [JsonProperty("event_slug")]
        public string EventSlug { get; set; }

        [JsonProperty("event_start_timestamp")]
        public long EventStartTimestamp { get; set; }

        [JsonProperty("event_track")]
        public string EventTrack { get; set; }

        [JsonProperty("event_track_id")]
        public long EventTrackId { get; set; }

        [JsonProperty("event_type")]
        public string EventType { get; set; }

        [JsonProperty("room_name")]
        public string RoomName { get; set; }

        [JsonProperty("room_slug")]
        public string RoomSlug { get; set; }

        [JsonProperty("speaker_list")]
        public SpeakerList[] SpeakerList { get; set; }

        [JsonProperty("tag_list")]
        public TagList[] TagList { get; set; }
    }

    public partial class SpeakerList
    {
        [JsonProperty("editions")]
        public Edition[] Editions { get; set; }

        [JsonProperty("speaker_city_list")]
        public SpeakerCityList[] SpeakerCityList { get; set; }

        [JsonProperty("speaker_company_name")]
        public string SpeakerCompanyName { get; set; }

        [JsonProperty("speaker_created_at")]
        public string SpeakerCreatedAt { get; set; }

        [JsonProperty("speaker_description")]
        public string SpeakerDescription { get; set; }

        [JsonProperty("speaker_id")]
        public long SpeakerId { get; set; }

        [JsonProperty("speaker_image")]
        public string SpeakerImage { get; set; }

        [JsonProperty("speaker_is_expert")]
        public bool SpeakerIsExpert { get; set; }

        [JsonProperty("speaker_is_listed")]
        public long SpeakerIsListed { get; set; }

        [JsonProperty("speaker_language")]
        public string SpeakerLanguage { get; set; }

        [JsonProperty("speaker_name")]
        public string SpeakerName { get; set; }

        [JsonProperty("speaker_order")]
        public long SpeakerOrder { get; set; }

        [JsonProperty("speaker_raw_image_path")]
        public string SpeakerRawImagePath { get; set; }

        [JsonProperty("speaker_slug")]
        public string SpeakerSlug { get; set; }

        [JsonProperty("speaker_tag_list")]
        public TagList[] SpeakerTagList { get; set; }

        [JsonProperty("speaker_title")]
        public string SpeakerTitle { get; set; }

        [JsonProperty("speaker_url")]
        public SpeakerUrl SpeakerUrl { get; set; }
    }

    public partial class SpeakerUrl
    {
        [JsonProperty("en")]
        public string En { get; set; }

        [JsonProperty("pl")]
        public string Pl { get; set; }
    }

    public partial class TagList
    {
        [JsonProperty("tag_id")]
        public long TagId { get; set; }

        [JsonProperty("tag_name")]
        public string TagName { get; set; }

        [JsonProperty("tag_parent_id")]
        public long TagParentId { get; set; }

        [JsonProperty("tag_slug")]
        public string TagSlug { get; set; }
    }

    public partial class SpeakerCityList
    {
        [JsonProperty("city_id")]
        public long CityId { get; set; }

        [JsonProperty("city_name")]
        public string CityName { get; set; }
    }

    public partial class Edition
    {
        [JsonProperty("edition_name")]
        public string EditionName { get; set; }
    }
    public partial class EventData
    {
        [JsonProperty("data")]
        public List<Event> Events { get; set; }
    }
    public partial class Event
    {
        public static IList<Event> FromJson(string json) => JsonConvert.DeserializeObject<EventData>(json, Converter.Settings).Events;
    }
    public class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
        };
    }


}