using MeetingApplication.Models;

namespace MeetingApplication.Services
{
    /// <summary>
    /// Class that handles all meeting related operations.
    /// </summary>
    public class MeetingService
    {
        private string _fileName { get; set; }
        private JsonService _jsonService;

        public MeetingService(string fileName)
        {
            _fileName = fileName;
            _jsonService = new JsonService(fileName);
        }

        /// <summary>
        /// Writes the meeting to the json file.
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public void CreateMeeting(Meeting meeting)
        {
            _jsonService.WriteMeetingToFile(meeting);
        }

        /// <summary>
        /// Deletes the meeting from the json file .
        /// </summary>
        /// <param name="id"></param>
        /// <param name="user"></param>
        /// <returns></returns>
        public int DeleteMeeting(int id, string user)
        {
            List<MeetingJson> newMeetings = new();
            var idFound = false;
            var response = 0;

            List<MeetingJson> meetings = _jsonService.ReadMeetingsFromFile();

            if (meetings == null)
                return response;

            if (meetings.Where(x => x.Meeting!.Id.Equals(id)).Select(x => x.Meeting!.ResponsiblePerson).FirstOrDefault() == user)
            {
                foreach (MeetingJson meeting in meetings)
                {
                    if (idFound)
                    {
                        meeting.Meeting!.Id -= 1;
                    }
                    else if (meeting.Meeting!.Id == id)
                    {
                        idFound = true;
                        response = 1;
                        continue;
                    }

                    newMeetings.Add(meeting);
                }

                _jsonService.WriteMeetingToFile(newMeetings);
            }
            else
            {
                response = -1;
            }

            return response;
        }

        /// <summary>
        /// Add the person to the meetings "Attendees" list.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        public bool AddPersonToMeeting(int meetingId, string person)
        {
            List<MeetingJson> meetings = _jsonService.ReadMeetingsFromFile();
            var isAdded = false;

            if (meetings == null)
                return isAdded;

            MeetingJson? meeting = meetings.Where(x => x.Meeting!.Id == meetingId).Select(x => x).FirstOrDefault();

            if (!meeting!.Meeting!.Attendees!.Contains(person))
            {
                isAdded = true;

                meeting!.Meeting!.Attendees!.Add(person);
                meetings.RemoveAt(meetingId);
                meetings.Insert(meetingId, meeting);

                _jsonService.WriteMeetingToFile(meetings);
            }

            return isAdded;
        }

        /// <summary>
        /// Removes the person from the "Attendees" list.
        /// </summary>
        /// <param name="meetingId"></param>
        /// <param name="person"></param>
        /// <returns></returns>
        public bool DeletePersonFromMeeting(int meetingId, string person)
        {

            List<MeetingJson> meetings = _jsonService.ReadMeetingsFromFile();
            var isDeleted = false;

            if (meetings == null)
                return isDeleted;

            MeetingJson? meeting = meetings.Where(x => x.Meeting!.Id == meetingId).Select(x => x).FirstOrDefault();

            if (meeting!.Meeting!.ResponsiblePerson!.ToLower() != person.ToLower())
            {
                meeting.Meeting.Attendees!.Remove(person);

                meetings.RemoveAt(meetingId);
                meetings.Insert(meetingId, meeting);
                _jsonService.WriteMeetingToFile(meetings);

                isDeleted = true;
            }

            return isDeleted;
        }

        /// <summary>
        /// Filters the list by description or the responsible person
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterKeyword"></param>
        /// <returns></returns>
        public List<MeetingJson>? ListMeetings(string? filter = null, string? filterKeyword = null)
        {
            List<MeetingJson> list = _jsonService.ReadMeetingsFromFile();

            if (list == null)
                return null;

            switch (filter)
            {
                case "description":
                    return list.Where(x => x.Meeting!.Description!.Contains(filterKeyword!)).Select(x => x).ToList();
                case "responsiblePerson":
                    return list.Where(x => x.Meeting!.ResponsiblePerson!.Contains(filterKeyword!)).Select(x => x).ToList();
                default:
                    return list;
            }
        }

        /// <summary>
        /// Filters the meeting by the category
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterKeyword"></param>
        /// <returns></returns>
        public List<MeetingJson>? ListMeetings(string? filter = null, Meeting.Categories? filterKeyword = null)
        {
            List<MeetingJson> list = _jsonService.ReadMeetingsFromFile();

            if (list == null)
                return null;

            if (filter == "category")
                return list.Where(x => x.Meeting!.Category!.Equals(filterKeyword)).Select(x => x).ToList();

            return list;
        }

        /// <summary>
        /// Filters the meeting by the type
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterKeyword"></param>
        /// <returns></returns>
        public List<MeetingJson>? ListMeetings(string? filter = null, Meeting.Types? filterKeyword = null)
        {
            List<MeetingJson> list = _jsonService.ReadMeetingsFromFile();

            if (list == null)
                return null;

            if (filter == "type")
                return list.Where(x => x.Meeting!.Type!.Equals(filterKeyword)).Select(x => x).ToList();

            return list;
        }

        /// <summary>
        /// Filters the meeting by date
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterKeyword"></param>
        /// <returns></returns>
        public List<MeetingJson>? ListMeetings(string? filter = null, DateTime? filterKeyword = null)
        {
            List<MeetingJson> list = _jsonService.ReadMeetingsFromFile();

            if (list == null)
                return null;

            if (filter == "dateFrom")
                return list.Where(x => x.Meeting!.StartDate >= filterKeyword).Select(x => x).ToList();

            return list;
        }

        /// <summary>
        /// Filters the meetings for the given range of dates
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterKeyword"></param>
        /// <returns></returns>
        public List<MeetingJson>? ListMeetings(string? filter = null, List<DateTime>? filterKeyword = null)
        {
            List<MeetingJson> list = _jsonService.ReadMeetingsFromFile();

            if (list == null)
                return null;

            if (filter == "dateBetween")
                return list.Where(x => (x.Meeting!.StartDate >= filterKeyword![0]) && (x.Meeting.EndDate <= filterKeyword[1].AddHours(23).AddMinutes(59).AddSeconds(59))).Select(x => x).ToList();

            return list;
        }

        /// <summary>
        /// Filters the meetings by the attendees count
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="filterKeyword"></param>
        /// <returns></returns>
        public List<MeetingJson>? ListMeetings(string? filter = null, int? filterKeyword = null)
        {
            List<MeetingJson> list = _jsonService.ReadMeetingsFromFile();

            if (list == null)
                return null;

            if (filter == "attendees")
                return list.Where(x => x.Meeting!.Attendees!.Count() >= filterKeyword).Select(x => x).ToList();

            return list;
        }

    }
}
