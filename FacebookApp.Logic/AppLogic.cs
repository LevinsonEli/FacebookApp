using System;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class AppLogic
    {
        private EventsPlanner m_EventsPlanner;
        private RealFriendsListFinder m_RealFriendsListFinder;
        private UserIterator m_LoggedInUser;

        public AppLogic(UserIterator i_LoggedInUser)
        {
            m_LoggedInUser = i_LoggedInUser;
            m_EventsPlanner = new EventsPlanner();
            m_RealFriendsListFinder = new RealFriendsListFinder();
        }

        public List<User> SearchRealFriendsList(UserIterator i_LoggedInUser)
        {
            return m_RealFriendsListFinder.SearchRealFriendsList(i_LoggedInUser);
        }

        public DateTime BirthdayStringToDateTime(User i_User)
        {
            return m_EventsPlanner.BirthdayStringToDateTime(i_User);
        }

        public string CreateEventLocationMapURL(Event i_Event)
        {
            return m_EventsPlanner.CreateEventLocationMapURL(i_Event);
        }
    }
}
