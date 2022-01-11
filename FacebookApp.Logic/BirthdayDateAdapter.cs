using System;
using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class BirthdayDateAdapter : IConvertableBirthday
    {
        private const int k_BirthdayDateShortFormatLength = 5;
        private const string k_BirthdayDateShortFormat = "MM/dd";
        private const string k_BirthdayDateLongFormat = "MM/dd/yyyy";
        private User m_User;

        public BirthdayDateAdapter(User i_User)
        {
            m_User = i_User;
        }

        public DateTime ConvertBirthdayToDate()
        {
            DateTime birthdayDateTime;
            if (m_User.Birthday != null)
            {
                if (m_User.Birthday.Length == k_BirthdayDateShortFormatLength)
                {
                    birthdayDateTime = DateTime.ParseExact(m_User.Birthday, k_BirthdayDateShortFormat, null);
                }
                else
                {
                    birthdayDateTime = DateTime.ParseExact(m_User.Birthday, k_BirthdayDateLongFormat, null);
                }

                return birthdayDateTime;
            }
            else
            {
                throw new NullReferenceException("Couldn't get data from facebook servers.");
            }
        }
    }
}
