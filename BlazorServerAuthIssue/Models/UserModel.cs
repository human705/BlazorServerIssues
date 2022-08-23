namespace BlazorServerAuthIssue.Models
{
    public class UserModel
    {
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserPassword { get; set; } = string.Empty;
        public int UserLevel { get; set; }
        public string userEmailAddress { get; set; } = string.Empty;
        public int UserStatus { get; set; }
        public string Userguid { get; set; } = string.Empty;
    }
}
