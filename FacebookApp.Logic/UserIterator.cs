using System;
using System.Collections;
using System.Collections.Generic;
using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class UserIterator : IEnumerable<User>
    {
        private readonly FacebookObjectCollection<User> r_Friends;
        private readonly List<IBirthdayObserver> r_BirthdayObservers = new List<IBirthdayObserver>();

        public User User { get; set; }

        public bool IsBirthdayToday { get; private set; }

        public UserIterator(User i_User)
        {
            User = i_User;
            r_Friends = User.Friends;
            IsBirthdayToday = false;
        }

        public IEnumerator<User> GetEnumerator()
        {
            foreach (User friend in r_Friends)
            {
                yield return friend;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public string FirstName
        {
            get { return User.FirstName; }
        }

        public string LastName
        {
            get { return User.LastName; }
        }

        public string Name
        {
            get { return User.Name; }
        }

        public string Birthday
        {
            get { return User.Birthday; }
        }

        public User.eGender? Gender
        {
            get { return User.Gender; }
        }

        public City Hometown
        {
            get { return User.Hometown; }
        }

        public City Location
        {
            get { return User.Location; }
        }

        public string Email
        {
            get { return User.Email; }
        }

        public FacebookObjectCollection<Event> Events
        {
            get { return User.Events; }
        }

        public FacebookObjectCollection<Page> LikedPages
        {
            get { return User.LikedPages; }
        }

        public FacebookObjectCollection<Post> Posts
        {
            get { return User.Posts; }
        }

        public FacebookObjectCollection<User> Friends
        {
            get { return User.Friends; }
        }

        public FacebookObjectCollection<Group> Groups
        {
            get { return User.Groups; }
        }

        public FacebookObjectCollection<Album> Albums
        {
            get { return User.Albums; }
        }

        public Status PostStatus(string i_StatusText)
        {
            return User.PostStatus(i_StatusText);
        }

        public string PictureNormalURL
        {
            get { return User.PictureNormalURL; }
        }

        public void CheckAndNotifyIfTodayIsMyBirthday()
        {
            string today = DateTime.Today.ToString("MM/dd");

            IsBirthdayToday = string.Equals(this.Birthday.Substring(0, 5), today);
            if (IsBirthdayToday)
            {
                notifyBirthdayObservers();
            }
        }

        private void notifyBirthdayObservers()
        {
            foreach (IBirthdayObserver observer in r_BirthdayObservers)
            {
                observer.ReportBirthdayIsToday(User);
            }
        }

        public void AddBirthdayObserver(IBirthdayObserver i_BirthdayObserver)
        {
            r_BirthdayObservers.Add(i_BirthdayObserver);
        }

        public void RemoveBirthdayObserver(IBirthdayObserver i_BirthdayObserver)
        {
            r_BirthdayObservers.Remove(i_BirthdayObserver);
        }
    }
}
