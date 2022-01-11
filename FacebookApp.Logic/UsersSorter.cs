using FacebookWrapper.ObjectModel;

namespace FacebookApp.Logic
{
    public class UsersSorter
    {
        public IUsersSortingStrategy UsersSortingStrategy { get; set; }

        public UsersSorter(IUsersSortingStrategy i_UsersSortingStrategy)
        {
            UsersSortingStrategy = i_UsersSortingStrategy;
        }

        private void swapUsers(ref User io_User1, ref User io_User2)
        {
            User tempUser = io_User1;
            io_User1 = io_User2;
            io_User2 = tempUser;
        }

        public void SortUsers(User[] i_Users)
        {
            for (int i = 0; i < i_Users.Length; i++)
            {
                for (int j = 0; j < i_Users.Length; j++)
                {
                    if (UsersSortingStrategy.ShouldSwapUsers(i_Users[i], i_Users[j]))
                    {
                        swapUsers(ref i_Users[i], ref i_Users[j]);
                    }
                }
            }
        }
    }
}
