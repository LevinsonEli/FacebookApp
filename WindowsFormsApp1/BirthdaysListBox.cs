using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using FacebookApp.Logic;
using FacebookWrapper.ObjectModel;

namespace FacebookApp.UI
{
    public class BirthdayListBox : ListBox, IBirthdayObserver
    {
        private UserIterator m_LoggedInUser;
        private List<UserIterator> m_FriendsList = new List<UserIterator>(); 

        public BirthdayListBox(UserIterator i_User)
        {
            initializeComponent();
            m_LoggedInUser = i_User;
            registerAsObserver();
        }

        private void initializeComponent()
        {
            this.Name = "FriendsBirthdaysListBox";
            this.DisplayMember = "Name";
            this.BackColor = Color.White;
            this.Font = new Font("Tempus Sans ITC", 9F);
            this.FormattingEnabled = true;
            this.ItemHeight = 24;
            this.Location = new Point(365, 52);
            this.Size = new Size(312, 76);
            this.TabIndex = 15;
        }

        public void ReportBirthdayIsToday(User i_User)
        {
            this.Items.Add(i_User);
        }

        private void registerAsObserver()
        {
            foreach (User friend in m_LoggedInUser)
            {
                UserIterator friendIterator = new UserIterator(friend);
                friendIterator.AddBirthdayObserver(this as IBirthdayObserver);
                m_FriendsList.Add(friendIterator);
                if (!string.IsNullOrEmpty(friendIterator.Birthday))
                {
                    friendIterator.CheckAndNotifyIfTodayIsMyBirthday();
                }
            }
        }
    }
}
