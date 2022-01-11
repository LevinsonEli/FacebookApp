using System;
using System.Text;
using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class EventsPlanner
    {
        public DateTime BirthdayStringToDateTime(User i_User)
        {
            BirthdayDateAdapter birthdayAdapter = new BirthdayDateAdapter(i_User);
            return birthdayAdapter.ConvertBirthdayToDate();
        }

        public string CreateEventLocationMapURL(Event i_Event)
        {
            StringBuilder eventLocationMapURL = new StringBuilder();

            eventLocationMapURL.Append("http://maps.google.com/maps?q=");

            if (!string.IsNullOrEmpty(i_Event.Place.Location.Street))
            {
                eventLocationMapURL.Append(i_Event.Place.Location.Street + ",+");
            }

            if (!string.IsNullOrEmpty(i_Event.Place.Location.City))
            {
                eventLocationMapURL.Append(i_Event.Place.Location.City + ",+");
            }

            if (!string.IsNullOrEmpty(i_Event.Place.Location.Country))
            {
                eventLocationMapURL.Append(i_Event.Place.Location.Country + ",+");
            }

            return eventLocationMapURL.ToString();
        }
    }
}
