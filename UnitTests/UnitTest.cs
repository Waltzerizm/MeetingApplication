using Newtonsoft.Json;

namespace UnitTests
{
    public class UnitTest
    {
        private JsonService _jsonService = new("test.json");
        private MeetingService _meetingService = new("test.json");

        [Fact]
        public void IsEqual_TestReadFromJson()
        {
            List<MeetingJson> expectedData = loadDummyData();
            List<MeetingJson> testData = _jsonService.ReadMeetingsFromFile();

            for (int i = 0; i < expectedData.Count; i++)
                Assert.Equal(JsonConvert.SerializeObject(expectedData[i]), JsonConvert.SerializeObject(testData[i]));
        }

        [Fact]
        public void IsEqual_TestCreateMeeting()
        {
            List<MeetingJson> expectedData = loadDummyData();

            Meeting newMeeting = new Meeting
            {
                Name = "Test .NET meeting!",
                ResponsiblePerson = "Rokas Lekecinskas",
                Description = ".NET meeting",
                Category = (Meeting.Categories)2,
                Type = (Meeting.Types)0,
                StartDate = DateTime.Parse("2022-07-22 10:30"),
                EndDate = DateTime.Parse("2022-07-22 11:00"),
                Attendees = new List<string> { "Rokas Lekecinskas" }
            };

            _meetingService.CreateMeeting(newMeeting);
            expectedData.Add(new MeetingJson { Meeting = newMeeting });

            List<MeetingJson> testData = _jsonService.ReadMeetingsFromFile();

            for (int i = 0; i < expectedData.Count; i++)
                Assert.Equal(JsonConvert.SerializeObject(expectedData[i]), JsonConvert.SerializeObject(testData[i]));
        }

        [Fact]
        public void IsEqual_TestDeleteMeeting()
        {
            List<MeetingJson> expectedData = loadDummyData();

            List<MeetingJson> newExpectedData = new();
            var idFound = false;

            foreach (MeetingJson meeting in expectedData)
            {
                if (idFound)
                {
                    meeting.Meeting!.Id -= 1;
                }
                else if (meeting.Meeting!.Id == 0)
                {
                    idFound = true;
                    continue;
                }

                newExpectedData.Add(meeting);
            }


            Assert.Equal(1, _meetingService.DeleteMeeting(0, "Rokas Lekecinskas")); // Check if the method returns "deleted successfully" response 

            var testData = _jsonService.ReadMeetingsFromFile();

            for (int i = 0; i < newExpectedData.Count; i++)
                Assert.Equal(JsonConvert.SerializeObject(newExpectedData[i]), JsonConvert.SerializeObject(testData[i]));
        }

        [Fact]
        public void Contains_AddPersonToMeeting()
        {
            loadDummyData();

            Assert.True(_meetingService.AddPersonToMeeting(0, "Naujenis Zmogenis"));

            var testData = _jsonService.ReadMeetingsFromFile();

            Assert.Contains("Naujenis Zmogenis", testData[0].Meeting!.Attendees);
            Assert.Equal(5, testData[0].Meeting!.Attendees!.Count());
        }

        [Fact]
        public void IsEqualAndContains_DeletePersonFromMeeting()
        {
            loadDummyData();

            Assert.True(_meetingService.DeletePersonFromMeeting(0, "Petras Petrauskas"));
            Assert.False(_meetingService.DeletePersonFromMeeting(0, "Rokas Lekecinskas"));

            var testData = _jsonService.ReadMeetingsFromFile();

            Assert.Contains("Rokas Lekecinskas", testData[0].Meeting!.Attendees);
            Assert.Contains("Mr. Direktor", testData[0].Meeting!.Attendees);
            Assert.Contains("Jurgis Puikulis", testData[0].Meeting!.Attendees);
            Assert.Equal(3, testData[0].Meeting!.Attendees!.Count());
        }

        [Fact]
        public void IsEqualAndContains_FilterMeetingsByDescription()
        {
            loadDummyData();

            var testData = _meetingService.ListMeetings("description", ".NET");
            var testMeeting = new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 2,
                    Name = "Another .NET meeting",
                    ResponsiblePerson = "Petras Petrauskas",
                    Description = "The inter did the .NET classic again",
                    Category = (Meeting.Categories)2,
                    Type = (Meeting.Types)1,
                    StartDate = DateTime.Parse("2022-07-23 10:30"),
                    EndDate = DateTime.Parse("2022-07-23 11:00"),
                    Attendees = new List<string> { "Petras Petrauskas", "Jurgis Puikulis", "Tiesiog Juozas", "Intern Internauskas" }
                }
            };

            Assert.Equal(3, testData.Count());
            Assert.Contains(JsonConvert.SerializeObject(testMeeting), JsonConvert.SerializeObject(testData));
        }

        [Fact]
        public void IsEqualAndContains_FilterMeetingsByCategory()
        {
            loadDummyData();

            var testData = _meetingService.ListMeetings("category", (Meeting.Categories)1);
            var testMeeting = new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 4,
                    Name = "Another .Net meeting!",
                    ResponsiblePerson = "Jurgis Puikulis",
                    Description = "Some important .NET stuff",
                    Category = (Meeting.Categories)1,
                    Type = (Meeting.Types)1,
                    StartDate = DateTime.Parse("2022-07-27 12:30"),
                    EndDate = DateTime.Parse("2022-07-27 13:00"),
                    Attendees = new List<string> { "Jurgis Puikulis", "Petras Petrauskas", "Tiesiog Juozas" }
                }
            };

            Assert.Equal(2, testData.Count());
            Assert.Contains(JsonConvert.SerializeObject(testMeeting), JsonConvert.SerializeObject(testData));
        }

        [Fact]
        public void IsEqual_FilterMeetingsByDate()
        {
            loadDummyData();

            var testData1 = _meetingService.ListMeetings("dateFrom", DateTime.Parse("2022-07-26"));
            var testData2 = _meetingService.ListMeetings("dateBetween", new List<DateTime> { DateTime.Parse("2022-07-22"), DateTime.Parse("2022-07-26") });

            Assert.Equal(2, testData1.Count());
            Assert.Equal(4, testData2.Count());
        }

        [Fact]
        public void IsEqual_FilterMeetingByAttendees()
        {
            loadDummyData();

            var testData = _meetingService.ListMeetings("attendees", 4);

            Assert.Equal(4, testData.Count());
        }

        public List<MeetingJson> loadDummyData()
        {
            List<MeetingJson> meetingJsons = new();

            meetingJsons.Add(new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 0,
                    Name = "ASAP .NET meeting!",
                    ResponsiblePerson = "Rokas Lekecinskas",
                    Description = ".NET meeting",
                    Category = (Meeting.Categories)2,
                    Type = (Meeting.Types)0,
                    StartDate = DateTime.Parse("2022-07-22 10:30"),
                    EndDate = DateTime.Parse("2022-07-22 11:00"),
                    Attendees = new List<string> { "Rokas Lekecinskas", "Petras Petrauskas", "Mr. Direktor", "Jurgis Puikulis" }
                }
            });
            meetingJsons.Add(new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 1,
                    Name = "Big super duper important meeting!",
                    ResponsiblePerson = "Mr. Direktor",
                    Description = "Buy high, sell low",
                    Category = (Meeting.Categories)1,
                    Type = (Meeting.Types)0,
                    StartDate = DateTime.Parse("2022-07-22 12:30"),
                    EndDate = DateTime.Parse("2022-07-22 13:00"),
                    Attendees = new List<string> { "Mr. Direktor", "Petras Petrauskas", "Jurgis Puikulis", "Tiesiog Juozas" }
                }
            });
            meetingJsons.Add(new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 2,
                    Name = "Another .NET meeting",
                    ResponsiblePerson = "Petras Petrauskas",
                    Description = "The inter did the .NET classic again",
                    Category = (Meeting.Categories)2,
                    Type = (Meeting.Types)1,
                    StartDate = DateTime.Parse("2022-07-23 10:30"),
                    EndDate = DateTime.Parse("2022-07-23 11:00"),
                    Attendees = new List<string> { "Petras Petrauskas", "Jurgis Puikulis", "Tiesiog Juozas", "Intern Internauskas" }
                }
            });
            meetingJsons.Add(new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 3,
                    Name = "Super fun teambuilding!",
                    ResponsiblePerson = "Mr. Direktor",
                    Description = "Fun time come here now now!",
                    Category = (Meeting.Categories)3,
                    Type = (Meeting.Types)1,
                    StartDate = DateTime.Parse("2022-07-26 12:30"),
                    EndDate = DateTime.Parse("2022-07-26 14:00"),
                    Attendees = new List<string> { "Mr. Direktor", "Petras Petrauskas", "Jurgis Puikulis", "Tiesiog Juozas" }
                }
            });
            meetingJsons.Add(new MeetingJson
            {
                Meeting = new Meeting
                {
                    Id = 4,
                    Name = "Another .Net meeting!",
                    ResponsiblePerson = "Jurgis Puikulis",
                    Description = "Some important .NET stuff",
                    Category = (Meeting.Categories)1,
                    Type = (Meeting.Types)1,
                    StartDate = DateTime.Parse("2022-07-27 12:30"),
                    EndDate = DateTime.Parse("2022-07-27 13:00"),
                    Attendees = new List<string> { "Jurgis Puikulis", "Petras Petrauskas", "Tiesiog Juozas" }
                }
            });

            _jsonService.WriteMeetingToFile(meetingJsons);

            return meetingJsons;
        }
    }
}