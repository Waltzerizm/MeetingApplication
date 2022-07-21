using MeetingApplication.Models;
using Newtonsoft.Json;


namespace MeetingApplication.Services
{
    /// <summary>
    /// Class that handles json file write/delete operations
    /// </summary>
    public class JsonService
    {

        public string FileName { get; set; }

        public JsonService(string fileName)
        {
            FileName = fileName;
        }

        /// <summary>
        /// Writes the meeting to the json file.
        /// </summary>
        /// <param name="meeting"></param>
        public void WriteMeetingToFile(Meeting meeting)
        {
            List<MeetingJson> list;

            if (IsTextFileEmpty(FileName))
            {
                list = new() { new MeetingJson { Meeting = meeting } };
            }
            else
            {
                list = ReadMeetingsFromFile();
                meeting.Id = list.Count;
                list.Add(new MeetingJson { Meeting = meeting });
            }

            var jsonString = JsonConvert.SerializeObject(list, Formatting.Indented);
            File.WriteAllText(FileName, jsonString);
        }

        /// <summary>
        /// Writes the list of meetings into the json file
        /// </summary>
        /// <param name="meetings"></param>
        public void WriteMeetingToFile(List<MeetingJson> meetings)
        {
            var jsonString = JsonConvert.SerializeObject(meetings, Formatting.Indented);
            File.WriteAllText(FileName, jsonString);
        }

        /// <summary>
        /// Reads the meetings from the json file
        /// </summary>
        /// <returns></returns>
        public List<MeetingJson> ReadMeetingsFromFile()
        {
            if(File.Exists(FileName))
            {
                string jsonString = File.ReadAllText(FileName);
                List<MeetingJson>? meetings = JsonConvert.DeserializeObject<List<MeetingJson>>(jsonString);
                return meetings!;
            }

            return null;
        }

        /// <summary>
        /// Checks if the file is empty
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public static bool IsTextFileEmpty(string fileName)
        {
            var info = new FileInfo(fileName);
            if (info.Length == 0)
                return true;

            if (info.Length < 6)
            {
                var content = File.ReadAllText(fileName);
                return content.Length == 0;
            }
            return false;
        }

    }
}
