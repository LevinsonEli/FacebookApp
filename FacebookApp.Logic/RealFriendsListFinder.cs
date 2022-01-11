using System;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class RealFriendsListFinder
    {
        private List<User> serachThisMonthFriendsBirthdays(List<User> i_FriendsWhoWroteOnWallForBirthday)
        {
            List<User> thisMonthBirthdayFriendList = new List<User>();

            foreach (User friend in i_FriendsWhoWroteOnWallForBirthday)
            {
                if (DateTime.Now.Month == getBirthdayMonth(friend.Birthday))
                {
                    thisMonthBirthdayFriendList.Add(friend);
                }
            }

            return thisMonthBirthdayFriendList;
        }

        private int getBirthdayMonth(string i_Birthday)
        {
            int birthdayMonth = (i_Birthday[0] - '0') * 10;

            birthdayMonth += i_Birthday[1] - '0';

            return birthdayMonth;
        }

        private int getBirthdayDay(string i_Birthday)
        {
            int birthdayDay = (i_Birthday[3] - '0') * 10;

            birthdayDay += i_Birthday[4] - '0';

            return birthdayDay;
        }

        private List<User> searchFriendsWhoWroteOnWallOnBirthday(UserIterator i_LoggedInUser)
        {
            List<User> wroteOnWallFriendsList = new List<User>();

            foreach (Post post in i_LoggedInUser.Posts)
            {
                if (post.CreatedTime.Value.Day == getBirthdayDay(i_LoggedInUser.Birthday) 
                    && post.CreatedTime.Value.Month == getBirthdayMonth(i_LoggedInUser.Birthday))
                {
                    wroteOnWallFriendsList.Add(post.From);
                }
            }

            return wroteOnWallFriendsList;
        }
        
        public List<User> SearchRealFriendsList(UserIterator i_LoggedInUser)
        {
            List<User> wroteOnWallFriendsList;
            List<User> hasBirthdayThisMonthFriendsList;
            
            wroteOnWallFriendsList = searchFriendsWhoWroteOnWallOnBirthday(i_LoggedInUser);
            hasBirthdayThisMonthFriendsList = serachThisMonthFriendsBirthdays(wroteOnWallFriendsList);

            return hasBirthdayThisMonthFriendsList;
        }
    }
}
