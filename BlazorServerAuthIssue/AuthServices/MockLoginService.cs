using Blazored.LocalStorage;
using BlazorServerAuthIssue.Models;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorServerAuthIssue.AuthServices
{
    public class MockLoginService : ILoginService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;
        private readonly ILocalStorageService _localStorageService;

        public MockLoginService(IUserRepository userRepository, IConfiguration configuration, ILocalStorageService localStorageService)
        {
            _userRepository = userRepository;
            _configuration = configuration;
            _localStorageService = localStorageService;
        }

        public async Task<UserModel> ValidateUserByJwt(string _token)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ValidateUserByPassword(string username, string password)
        {
            Log.Information("Validating user creds!");
            try
            {
                var loggedInUser = await _userRepository.GetUserByPassword(username, password);
                if (loggedInUser != null)
                {
                    var retToken = GenerateJwtToken(loggedInUser);
                    if (retToken != string.Empty)
                    {
                        return retToken;
                    }
                    return string.Empty;

                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                //Log.Fatal($"Excsption : {ex.Message.ToString()}");
                return string.Empty;
            }

        }

        public string GenerateJwtToken(UserModel user)
        {
            Log.Information("Generating JWT token");
            try
            {
                //getting the secret key
                string secretKey = _configuration["JWTSettings:SecretKey"];
                if (secretKey == null)
                {
                    Log.Warning("JWT Secret key is empty");
                    return string.Empty;
                }
                var key = Encoding.ASCII.GetBytes(secretKey);
                // TODO - Add Role claim
                //create claims
                var claimEmail = new Claim(ClaimTypes.Email, user.userEmailAddress.ToString());
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, user.UserName.ToString());
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmail, claimNameIdentifier }, "serverAuth");
                // generate token that is valid for 2 hours
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = claimsIdentity,
                    Expires = DateTime.UtcNow.AddHours(2),
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                };
                //creating a token handler
                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);
                //returning the token back
                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                Log.Fatal($"Exception: {ex.Message.ToString()}");
                return string.Empty;
            }
        }

        public async Task<UserModel> ValidateUserUserName(string username)
        {
            Log.Information("Validating user with username!");
            try
            {
                var loggedInUser = await _userRepository.GetUserByUserName(username);
                if (loggedInUser != null)
                {
                    return loggedInUser;
                }
                return null;
            }
            catch (Exception ex)
            {
                Log.Fatal($"Exception : {ex.Message.ToString()}");
                return null;
            }
        }
    }
}
