using BlazorServerAuthIssue.Models;

namespace BlazorServerAuthIssue.AuthServices
{
    public interface ILoginService
    {
        Task<UserModel> ValidateUserByJwt(string _token);
        Task<string> ValidateUserByPassword(string username, string password);
        Task<UserModel> ValidateUserUserName(string username);
        public string GenerateJwtToken(UserModel user);
    }
}
