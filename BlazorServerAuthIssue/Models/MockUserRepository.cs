namespace BlazorServerAuthIssue.Models
{
    public class MockUserRepository : IUserRepository
    {
        private List<UserModel> _userList;

        public MockUserRepository()
        {
            _userList = new List<UserModel>
            {
                new UserModel() {UserId = 0, UserName = "user1", UserPassword = "test", UserLevel = 10, userEmailAddress = "me@here.com", UserStatus  = 1, Userguid = ""},
                new UserModel() {UserId = 0, UserName = "user2", UserPassword = "test", UserLevel = 10, userEmailAddress = "me@here.com", UserStatus  = 1, Userguid = ""},
            };
        }

        public Task<int> AddAsync(UserModel entity)
        {
            throw new NotImplementedException();
        }

        public Task<int> DeleteAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<List<UserModel>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<UserModel> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateAsync(UserModel entity)
        {
            throw new NotImplementedException();
        }

        public async Task<UserModel> GetUserByPassword(string u, string p)
        {
            var v = _userList.FirstOrDefault(e => e.UserName == u && e.UserPassword == p);
            if (v != null)
            {
                return v;
            }
            return null;
        }

        public async Task<UserModel> GetUserByUserName(string u)
        {
            return (_userList.FirstOrDefault(e => e.UserName == u) != null) ? _userList.FirstOrDefault(e => e.UserName == u) : null;
        }

        public List<UserModel> GetAllTest()
        {
            return _userList.ToList();
        }
    }
}
