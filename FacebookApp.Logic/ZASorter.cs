using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class ZASorter : IUsersSortingStrategy
    {
        public bool ShouldSwapUsers(User i_User1, User i_User2)
        {
            return string.Compare(i_User1.Name, i_User2.Name) < 0;
        }
    }
}
