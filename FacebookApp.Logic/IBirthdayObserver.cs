using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public interface IBirthdayObserver
    {
        void ReportBirthdayIsToday(User i_User);
    }
}
