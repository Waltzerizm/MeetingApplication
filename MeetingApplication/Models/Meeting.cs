using System.Text;
using System.Text.Json;
using Newtonsoft.Json;

namespace MeetingApplication.Models
{
    public class Meeting
    {
       [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("responsiblePerson")]
        public string? ResponsiblePerson { get; set; }
        [JsonProperty("description")]
        public string? Description { get; set; }
        [JsonProperty("category")]
        public Categories Category { get; set; }
        [JsonProperty("type")]
        public Types Type { get; set; }
        [JsonProperty("starDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("attendees")]
        public List<string>? Attendees { get; set; }

        public enum Categories { CodeMonkey, Hub, Short, TeamBuilding }
        public enum Types { Live, InPerson }

        public override string ToString()
        {
            StringBuilder attendees = new();

            foreach(var attendee in Attendees!)
                attendees.Append(attendee.ToString() + ", ");

            return $"Meeting Nr.{Id}:\n" + $" Name: {Name},\n ResponsiblePerson: {ResponsiblePerson}" +
                $",\n Description: {Description},\n Category: {Category},\n Type: {Type},\n StartDate: {StartDate.ToString("dddd, dd MMMM yyyy - HH:mm")}" +
                $",\n EndDate: {EndDate.ToString("dddd, dd MMMM yyyy - HH:mm")},\n Attendees: {attendees}.\n";
        }

    }
}
