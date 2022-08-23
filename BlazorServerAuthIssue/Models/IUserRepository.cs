namespace BlazorServerAuthIssue.Models
{
    public interface IUserRepository : IGenericRepository<UserModel>
    {
        public Task<UserModel> GetUserByPassword(string _username, string _password);
        public Task<UserModel> GetUserByUserName(string _username);
    }
}
