using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Forms;
using FacebookWrapper;
using FacebookWrapper.ObjectModel;
using FacebookApp.Logic;

namespace FacebookApp.UI
{
    public partial class AppForm : Form
    {
        private static AppForm s_AppFormInstance = null;
        private static object sr_FetchInfoLock = new object();
        private UserIterator m_LoggedInUser;
        private AppLogic m_AppLogic;

        public AppForm()
        {
            InitializeComponent();
        }

        public static AppForm AppFormInstance
        {
            get
            {
                if (s_AppFormInstance != null)
                {
                    return s_AppFormInstance;
                }

                return null;
            }
        }

        public static AppForm Create()
        {
            if (s_AppFormInstance == null)
            {
                lock (sr_FetchInfoLock)
                {
                    if (s_AppFormInstance == null)
                    {
                        s_AppFormInstance = new AppForm();
                    }
                }
            }

            return s_AppFormInstance;
        }

        private void buttonLogin_Click(object sender, EventArgs e)
        {
            loginAndInit();
        }
        
        private void loginAndInit()
        {
            LoginResult result = FacebookService.Login("290696395492712",
                "public_profile",
                "email",
                "publish_to_groups",
                "user_birthday",
                "user_age_range",
                "user_gender",
                "user_link",
                "user_tagged_places",
                "user_videos",
                "publish_to_groups",
                "groups_access_member_info",
                "user_friends",
                "user_events",
                "user_likes",
                "user_location",
                "user_photos",
                "user_posts",
                "user_hometown"
                );

            if (!string.IsNullOrEmpty(result.AccessToken))
            {
                m_LoggedInUser = new UserIterator(result.LoggedInUser);
                m_AppLogic = new AppLogic(m_LoggedInUser);
                fetchUserInfoAndPosts();
            }
            else
            {
                MessageBox.Show(result.ErrorMessage);
            }
        }

        private void fetchUserInfoAndPosts()
        {
            Thread fetchUserInfoThread = new Thread(fetchUserInfo);
            Thread fetchUserPostsThread = new Thread(fetchPosts);
            fetchUserInfoThread.Start();
            fetchUserPostsThread.Start();
            createAndInitBirthdayListBox();
        }

        private void createAndInitBirthdayListBox()
        {
            BirthdayListBox friendsBirthdaysListBox = new BirthdayListBox(m_LoggedInUser);
            this.panelTodaysBirthdays.Controls.Add(friendsBirthdaysListBox);
        }

        private void buttonLogout_Click(object sender, EventArgs e)
        {
            FacebookService.Logout(clearUser);
        }

        private void clearUser()
        {
            pictureBoxUserPicture.ImageLocation = null;
            listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Clear()));
        }

        private void fetchUserInfo()
        {
            if (m_LoggedInUser != null)
            {
                lock (sr_FetchInfoLock)
                {
                    if (m_LoggedInUser != null)
                    {
                        pictureBoxUserPicture.LoadAsync(m_LoggedInUser.PictureNormalURL);
                        labelUserName.Invoke(new Action(() => labelUserName.Text = m_LoggedInUser.FirstName));
                        labelUserLastName.Invoke(new Action(() => labelUserLastName.Text = m_LoggedInUser.LastName));
                    }
                    else
                    {
                        showErrorMessageNoLoggedInUser();
                    }
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void fetchPosts()
        {
            if (m_LoggedInUser != null)
            {
                lock (sr_FetchInfoLock)
                {
                    if (m_LoggedInUser != null)
                    {
                        listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Clear()));
                        
                        foreach (Post post in m_LoggedInUser.Posts)
                        {
                            if (post.Message != null)
                            {
                                listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Add(post.Message)));
                            }
                            else if (post.Caption != null)
                            {
                                listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Add(post.Caption)));
                            }
                            else
                            {
                                listBoxPosts.Invoke(new Action(() => listBoxPosts.Items.Add(string.Format("{0}", post.Type))));
                            }
                        }

                        if (m_LoggedInUser.Posts.Count == 0)
                        {
                            MessageBox.Show("...No Posts...");
                        }
                    }
                    else
                    {
                        showErrorMessageNoLoggedInUser();
                    }
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void buttonMakeNewPost_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser != null)
            {
                try
                {
                    Status newStatus = m_LoggedInUser.PostStatus(textBoxNewStatus.Text);
                }
                catch
                {
                    MessageBox.Show("Unknown Error. Can't post your status. ");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }
        
        private void showErrorMessageNoLoggedInUser()
        {
            MessageBox.Show("Login first");
        }

        private void buttonPosts_Click(object sender, EventArgs e)
        {
            fetchPosts();
        }

        private void buttonFriends_Click(object sender, EventArgs e)
        {
            listBoxFriends.Items.Clear();

            foreach (User friend in m_LoggedInUser.Friends)
            {
                listBoxFriends.Items.Add(friend.ToString() + "\n");
                friend.ReFetch(DynamicWrapper.eLoadOptions.Full);
            }

            if (m_LoggedInUser.Friends.Count == 0)
            {
                MessageBox.Show("No Friends to retrieve :(");
            }
        }
       
        // Feature num 1
        private void buttonSearchFriends_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser != null)
            {
                List<User> friendsWhoCelebrateBirthdayAndWishedHB = m_AppLogic.SearchRealFriendsList(m_LoggedInUser);

                foreach (User friend in friendsWhoCelebrateBirthdayAndWishedHB)
                {
                    listBoxBirthdays.Items.Add(friend.Name);
                }

                if (listBoxBirthdays.Items.Count == 0)
                {
                    MessageBox.Show("No results");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void buttonWishAHappyBirthday_Click(object sender, EventArgs e)
        {
            if (m_LoggedInUser != null)
            {
                foreach (string friendName in listBoxBirthdays.SelectedItems)
                {
                    foreach (User friend in m_LoggedInUser.Friends)
                    {
                        if (friendName.Equals(friend.Name))
                        {
                            friend.PostStatus("Happy Birthday, {0}, my best friend!", friendName);
                            string publishedMessage = string.Format("you wrote on {0} wall!", friendName);
                            MessageBox.Show(publishedMessage);
                        }
                    }
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        // Feature num 2
        private void updateMonthCalendarBoldedDates()
        {
            if (m_LoggedInUser != null)
            {
                listBoxEvents.DisplayMember = "Name";

                foreach (Event facebookEvent in m_LoggedInUser.Events)
                {
                    monthCalendarEventsPlanner.AddBoldedDate((DateTime)facebookEvent.StartTime);
                }

                foreach (User friend in m_LoggedInUser.Friends)
                {
                    monthCalendarEventsPlanner.AddBoldedDate(m_AppLogic.BirthdayStringToDateTime(friend));
                }

                monthCalendarEventsPlanner.UpdateBoldedDates();
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void monthCalendarEventsPlanner_DateSelected(object sender, DateRangeEventArgs e)
        {
            if (m_LoggedInUser != null)
            {
                foreach (Event facebookEvent in m_LoggedInUser.Events)
                {
                    if (monthCalendarEventsPlanner.SelectionStart == facebookEvent.StartTime)
                    {
                        listBoxEvents.Items.Add(facebookEvent);
                    }
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void listBoxEventsOnThisDay_SelectedIndexChanged(object sender, EventArgs e)
        {
            createEventLocationMapURL(listBoxEvents.SelectedItem as Event);
        }

        private void createEventLocationMapURL(Event i_Event)
        {
            string eventLocationURL = m_AppLogic.CreateEventLocationMapURL(i_Event);

            webBrowserEventLocation.Navigate(eventLocationURL);
        }

        private void buttonFetchPosts_Click(object sender, EventArgs e)
        {
            fetchPosts();
        }

        private void buttonFetchFriends_Click(object sender, EventArgs e)
        {
            fetchFriends();
        }

        private void fetchFriends()
        {
            if (m_LoggedInUser != null)
            {
                listBoxFriends.Invoke(new Action(() => listBoxFriends.Items.Clear()));
                listBoxFriends.Invoke(new Action(() => listBoxFriends.DisplayMember = "Name"));

                foreach (User friend in m_LoggedInUser.Friends)
                {
                    listBoxFriends.Invoke(new Action(() => listBoxFriends.Items.Add(friend)));
                    friend.ReFetch(DynamicWrapper.eLoadOptions.Full);
                }

                if (m_LoggedInUser.Friends.Count == 0)
                {
                    MessageBox.Show("...No Friends...");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void listBoxFriends_SelectedIndexChanged(object sender, EventArgs e)
        {
            displayFriendInfo();
        }

        private void displayFriendInfo()
        {
            User selectedFriend;

            if (m_LoggedInUser != null)
            {
                if (listBoxFriends.SelectedItems.Count == 1)
                {
                    selectedFriend = listBoxFriends.SelectedItem as User;
                    labelSelectedFriendsNameData.Text = selectedFriend.Name;
                    fetchFriendsBirthday(selectedFriend);
                    fetchFriendsEmail(selectedFriend);
                    fetchFriendsProfilePic(selectedFriend);
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void fetchFriendsProfilePic(User i_Friend)
        {
            if (i_Friend.PictureNormalURL != null)
            {
                pictureBoxSelectedFriend.LoadAsync(i_Friend.PictureNormalURL);
            }
            else
            {
                pictureBoxSelectedFriend.Image = pictureBoxSelectedFriend.ErrorImage;
            }
        }

        private void fetchFriendsEmail(User i_Friend)
        {
            if (i_Friend.Email != null)
            {
                labelSelectedFriendsEmailData.Text = i_Friend.Email;
            }
            else
            {
                labelSelectedFriendsEmailData.Text = "Unavailable";
            }
        }

        private void fetchFriendsBirthday(User i_Friend)
        {
            if (i_Friend.Birthday != null)
            {
                labelSelectedFriendsBirthdayData.Text = i_Friend.Birthday.ToString();
            }
            else
            {
                labelSelectedFriendsBirthdayData.Text = "Unavailable";
            }
        }

        private void buttonFetchAlbums_Click(object sender, EventArgs e)
        {
            listBoxAlbums.Items.Clear();
            listBoxAlbums.DisplayMember = "Name";

            if (m_LoggedInUser != null)
            {
                foreach (Album album in m_LoggedInUser.Albums)
                {
                    listBoxAlbums.Items.Add(album);
                }

                if (m_LoggedInUser.Albums.Count == 0)
                {
                    MessageBox.Show("...No Albums...");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void listBoxAlbums_SelectedIndexChanged(object sender, EventArgs e)
        {
            Album currentAlbum = listBoxAlbums.SelectedItem as Album;

            clearPictureBoxesOfPhotoAlbum();

            if (currentAlbum.Photos.Count != 0)
            {
                updatePhotosOfChosenAlbum(currentAlbum);
            }
            else
            {
                MessageBox.Show("...No Photos...");
            }
        }

        private void clearPictureBoxesOfPhotoAlbum()
        {
            foreach (PictureBox picture in panelAlbumPhotoes.Controls)
            {
                picture.Image = null;
            }
        }

        private void updatePhotosOfChosenAlbum(Album i_Album)
        {
            for (int i = 0; i < panelAlbumPhotoes.Controls.Count && i < i_Album.Count; i++)
            {
                (panelAlbumPhotoes.Controls[i] as PictureBox).ImageLocation = GetNextPhotoURL(i_Album, i);
                (panelAlbumPhotoes.Controls[i] as PictureBox).SizeMode = PictureBoxSizeMode.StretchImage;
            }
        }

        public string GetNextPhotoURL(Album i_Album, int i_Index)
        {
            string nextPhotoURL;
            
            if (i_Album.Photos.Count != (i_Index + 1) && i_Album.Photos[i_Index + 1] != null)
            {
                nextPhotoURL = i_Album.Photos[i_Index + 1].URL;
            }
            else
            {
                nextPhotoURL = null;
            }

            return nextPhotoURL;
        }

        private void buttonFetchPages_Click(object sender, EventArgs e)
        {
            fetchLikedPages();
        }

        private void fetchLikedPages()
        {
            listBoxPages.Items.Clear();
            listBoxPages.DisplayMember = "Name";

            if (m_LoggedInUser != null)
            {
                try
                {
                    foreach (Page page in m_LoggedInUser.LikedPages)
                    {
                        listBoxPages.Items.Add(page);
                    }

                    if (m_LoggedInUser.LikedPages.Count == 0)
                    {
                        MessageBox.Show("...No Liked Pages...");
                    }
                }
                catch
                {
                    MessageBox.Show("Unknown Error. Can't fetch pages.");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void buttonFetchGroups_Click(object sender, EventArgs e)
        {
            fetchGroups();
        }

        private void fetchGroups()
        {
            listBoxGroups.Items.Clear();
            listBoxGroups.DisplayMember = "Name";

            if (m_LoggedInUser != null)
            {
                foreach (Group group in m_LoggedInUser.Groups)
                {
                    listBoxGroups.Items.Add(group);
                }

                if (m_LoggedInUser.Groups.Count == 0)
                {
                    MessageBox.Show("...No Groups...");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }

        private void buttonAZSortFrineds_Click(object sender, EventArgs e)
        {
            fetchAndSortFriends(new AZSorter());
        }

        private void buttonZASortFriends_Click(object sender, EventArgs e)
        {
            fetchAndSortFriends(new ZASorter());
        }

        private void fetchAndSortFriends(IUsersSortingStrategy i_UsersFortingStrategy)
        {
            if (m_LoggedInUser != null)
            {
                listBoxFriends.Items.Clear();
                listBoxFriends.DisplayMember = "Name";

                User[] friendsArray = new User[m_LoggedInUser.Friends.Count];

                m_LoggedInUser.Friends.CopyTo(friendsArray, 0);
                UsersSorter usersSorter = new UsersSorter(i_UsersFortingStrategy);

                usersSorter.SortUsers(friendsArray);

                foreach (User friend in friendsArray)
                {
                    listBoxFriends.Items.Add(friend);
                    friend.ReFetch(DynamicWrapper.eLoadOptions.Full);
                }

                if (friendsArray.Length == 0)
                {
                    MessageBox.Show("...No Friends...");
                }
            }
            else
            {
                showErrorMessageNoLoggedInUser();
            }
        }
    }
}
