using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeetingApplication.Models;
using MeetingApplication.Services;

namespace MeetingApplication
{
    public class Application
    {

        private MeetingService _meetingService = new("meetingsLog.json");
        private string? _User { get; set; }

        /// <summary>
        /// Authenticates the user by asking their full name for managing the meetings
        /// </summary>
        public void Authenticate()
        {
            Console.WriteLine("Welcome to the Visma meeting managment system");
            Console.WriteLine("Input your full name for authentication");
            Console.Write("User> ");
            _User = Console.ReadLine();

            while (_User == "")
            {
                Console.WriteLine("Empty input, try again");
                _User = Console.ReadLine();
            }

            Console.WriteLine("Welcome " + _User + "!\n");
        }


        /// <summary>
        /// Main application, handles I/O and function calls
        /// </summary>
        public void Run()
        {
            Authenticate();

            Console.WriteLine("Meeting manager commands:");
            Console.WriteLine("    '1' - Create a new meeting");
            Console.WriteLine("    '2' - Delete a meeting");
            Console.WriteLine("    '3' - Add person to a meeting");
            Console.WriteLine("    '4' - Delete person from a meeting");
            Console.WriteLine("    '5' - List meetings");
            Console.WriteLine("    'x' - Close application\n");

            while (true)
            {
                Console.Write(_User + "> ");
                var userInput = Console.ReadLine();

                if (userInput!.ToString() == "1") // CREATE MEETING
                {
                    var newMeeting = new Meeting();
                    int enumIndexInput;
                    DateTime dateInput;

                    Console.WriteLine("System> Input meeting name: ");
                    Console.Write(_User + "> ");
                    newMeeting.Name = Console.ReadLine();



                    newMeeting.ResponsiblePerson = _User;
                    List<string> attendees = new();
                    attendees.Add(_User!);
                    newMeeting.Attendees = attendees;



                    Console.WriteLine("System> Input meeting description: ");
                    Console.Write(_User + "> ");
                    newMeeting.Description = Console.ReadLine();



                    Console.WriteLine("System> Input meeting category (0 - CodeMonkey, 1 - Hub, 2 - Short, 3 - TeamBuilding): ");
                    Console.Write(_User + "> ");
                    while (true)
                    {
                        if (!Int32.TryParse(Console.ReadLine(), out enumIndexInput))
                        {
                            Console.WriteLine("System> Invalid category, try again");
                            Console.Write(_User + "> ");
                        }
                        else if (enumIndexInput is >= 0 and <= 3)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("System> Invalid category, try again");
                            Console.Write(_User + "> ");
                        }
                    }
                    newMeeting.Category = (Meeting.Categories)enumIndexInput;



                    Console.WriteLine("Input meeting type (0 - Live, 1 - InPerson): ");
                    while (true)
                    {
                        if (!Int32.TryParse(Console.ReadLine(), out enumIndexInput))
                        {
                            Console.WriteLine("System> Invalid type, try again");
                            Console.Write(_User + "> ");
                        }
                        else if (enumIndexInput is >= 0 and <= 1)
                        {
                            break;
                        }
                        else
                        {
                            Console.WriteLine("System> Invalid type, try again");
                            Console.Write(_User + "> ");
                        }
                    }
                    newMeeting.Type = (Meeting.Types)enumIndexInput;



                    Console.WriteLine("Input meeting start date (YYYY-MM-DD HH:mm): ");
                    Console.Write(_User + "> ");
                    while (!DateTime.TryParse(Console.ReadLine(), out dateInput))
                    {
                        Console.WriteLine("System> Invalid start date, try again");
                        Console.Write(_User + "> ");
                    }
                    newMeeting.StartDate = dateInput;



                    Console.WriteLine("Input meeting end date (YYYY-MM-DD HH:mm): ");
                    Console.Write(_User + "> ");
                    while (!DateTime.TryParse(Console.ReadLine(), out dateInput))
                    {
                        Console.WriteLine("System> Invalid end date, try again");
                        Console.Write(_User + "> ");
                    }
                    newMeeting.EndDate = dateInput;


                    _meetingService.CreateMeeting(newMeeting);

                    Console.WriteLine("Meeting has been created");
                }
                else if (userInput!.ToString() == "2") // DELETE MEETING
                {
                    int id;

                    Console.WriteLine("Input the meeting ID that you want to delete: ");
                    while (!Int32.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("System> Invalid input, try again");
                        Console.Write(_User + "> ");
                    }

                    var response = _meetingService.DeleteMeeting(id, _User!);

                    if (response == 1)
                        Console.WriteLine("Successfully deleted");
                    else if (response == 0)
                        Console.WriteLine("Error occurred, try again");
                    else
                        Console.WriteLine("Only the response person can delete this meeting");
                }
                else if (userInput!.ToString() == "3") // ADD PERSON
                {
                    int id;

                    Console.WriteLine("Input the ID of the meeting: ");
                    while (!Int32.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("System> Invalid input, try again");
                        Console.Write(_User + "> ");
                    }

                    Console.WriteLine("Input the persons name: ");
                    var name = Console.ReadLine();

                    if (_meetingService.AddPersonToMeeting(id, name!))
                    {
                        Console.WriteLine("Person succesfully added");
                    }
                    else
                    {
                        Console.WriteLine("Meeting not found, try again");
                    }
                }
                else if (userInput!.ToString() == "4") // DELETE PERSON
                {
                    int id;

                    Console.WriteLine("Input the ID of the meeting: ");
                    while (!Int32.TryParse(Console.ReadLine(), out id))
                    {
                        Console.WriteLine("System> Invalid input, try again");
                        Console.Write(_User + "> ");
                    }

                    Console.WriteLine("Input the persons name: ");
                    var name = Console.ReadLine();

                    if (_meetingService.DeletePersonFromMeeting(id, name!))
                    {
                        Console.WriteLine("Person succesfully deleted");
                    }
                    else
                    {
                        Console.WriteLine("Meeting or person not found, try again");
                    }


                }
                else if (userInput!.ToString() == "5") // FILTER
                {
                    string keyword;
                    List<MeetingJson>? meetings;

                    Console.WriteLine("Filter options:");
                    Console.WriteLine("    'enter' - List all meethings without filtering");
                    Console.WriteLine("    '1' - Filter by DESCRIPTION");
                    Console.WriteLine("    '2' - Filter by RESPONSIBLE PERSON");
                    Console.WriteLine("    '3' - Filter by CATEGORY");
                    Console.WriteLine("    '4' - Filter by TYPE");
                    Console.WriteLine("    '5' - Filter by DATE");
                    Console.WriteLine("    '6' - Filter by ATTENDEE COUNT\n");
                    Console.Write(_User + "> ");

                    switch (Console.ReadLine())
                    {
                        case "1":
                            Console.WriteLine("Enter filter keyword: ");
                            Console.Write(_User + "> ");
                            keyword = Console.ReadLine()!;
                            meetings = _meetingService.ListMeetings("description", keyword);
                            break;
                        case "2":
                            Console.WriteLine("Enter filter keyword: ");
                            Console.Write(_User + "> ");
                            keyword = Console.ReadLine()!;
                            meetings = _meetingService.ListMeetings("responsiblePerson", keyword);
                            break;
                        case "3":
                            Console.WriteLine("Enter filter keyword (0 - CodeMonkey, 1 - Hub, 2 - Short, 3 - TeamBuilding): ");
                            Console.Write(_User + "> ");

                            int categoryKeyword;

                            while (true)
                            {
                                if (!Int32.TryParse(Console.ReadLine(), out categoryKeyword))
                                {
                                    Console.WriteLine("System> Invalid category, try again");
                                    Console.Write(_User + "> ");
                                }
                                else if (categoryKeyword is >= 0 and <= 3)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("System> Invalid category, try again");
                                    Console.Write(_User + "> ");
                                }
                            }

                            meetings = _meetingService.ListMeetings("category", (Meeting.Categories)categoryKeyword);
                            break;
                        case "4":
                            Console.WriteLine("Enter filter keyword (0 - Live, 1 - InPerson): ");
                            Console.Write(_User + "> ");

                            int typeKeyword;

                            while (true)
                            {
                                if (!Int32.TryParse(Console.ReadLine(), out typeKeyword))
                                {
                                    Console.WriteLine("System> Invalid type, try again");
                                    Console.Write(_User + "> ");
                                }
                                else if (typeKeyword is >= 0 and <= 1)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("System> Invalid type, try again");
                                    Console.Write(_User + "> ");
                                }
                            }

                            meetings = _meetingService.ListMeetings("type", (Meeting.Types)typeKeyword);
                            break;
                        case "5":
                            Console.WriteLine("Enter date filter type (0 - show all from start date, 1 - show all from start date to end date)");
                            Console.Write(_User + "> ");

                            int dateKeyword;


                            while (true)
                            {
                                if (!Int32.TryParse(Console.ReadLine(), out dateKeyword))
                                {
                                    Console.WriteLine("System> Invalid type, try again");
                                    Console.Write(_User + "> ");
                                }
                                else if (dateKeyword is >= 0 and <= 1)
                                {
                                    break;
                                }
                                else
                                {
                                    Console.WriteLine("System> Invalid type, try again");
                                    Console.Write(_User + "> ");
                                }
                            }

                            if (dateKeyword == 0)
                            {
                                DateTime startDateKeyword;

                                Console.WriteLine("Enter filter start date (YYYY-MM-DD): ");
                                Console.Write(_User + "> ");

                                while (!DateTime.TryParse(Console.ReadLine(), out startDateKeyword))
                                {
                                    Console.WriteLine("System> Invalid end date, try again");
                                    Console.Write(_User + "> ");
                                }

                                meetings = _meetingService.ListMeetings("date", startDateKeyword);
                                break;
                            }
                            else
                            {
                                List<DateTime> dates = new();
                                DateTime startDateKeyword;
                                DateTime endDateKeyword;

                                Console.WriteLine("Enter filter start date (YYYY-MM-DD): ");
                                Console.Write(_User + "> ");

                                while (!DateTime.TryParse(Console.ReadLine(), out startDateKeyword))
                                {
                                    Console.WriteLine("System> Invalid start date, try again");
                                    Console.Write(_User + "> ");
                                }

                                dates.Add(startDateKeyword);

                                Console.WriteLine("Enter filter end date (YYYY-MM-DD): ");
                                Console.Write(_User + "> ");

                                while (!DateTime.TryParse(Console.ReadLine(), out endDateKeyword))
                                {
                                    Console.WriteLine("System> Invalid end date, try again");
                                    Console.Write(_User + "> ");
                                }

                                dates.Add(endDateKeyword);

                                meetings = _meetingService.ListMeetings("date", dateKeyword);
                                break;
                            }
                        case "6":
                            Console.WriteLine("Enter attendee count: ");
                            Console.Write(_User + "> ");

                            int attendeesKeyword;

                            while (!Int32.TryParse(Console.ReadLine(), out attendeesKeyword))
                            {
                                Console.WriteLine("System> Invalid input, try again");
                                Console.Write(_User + "> ");
                            }

                            meetings = _meetingService.ListMeetings("attendees", attendeesKeyword);
                            break;
                        default:
                            meetings = _meetingService.ListMeetings("default", "default");
                            break;
                    }

                    if (meetings != null && meetings?.Count() != 0)
                    {
                        foreach (MeetingJson meeting in meetings!)
                        {
                            Console.WriteLine(meeting.Meeting?.ToString());
                        }
                    }
                    else
                        Console.WriteLine("List is empty");
                }
                else if (userInput!.ToString() == "x")
                {
                    break;
                }
                else
                {
                    Console.WriteLine("Invalid input");
                }
            }
        }
    }
}
