using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public interface IUsersSortingStrategy
    {
        bool ShouldSwapUsers(User i_User1, User i_User2);
    }
}
