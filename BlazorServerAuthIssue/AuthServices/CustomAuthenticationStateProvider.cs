using Blazored.LocalStorage;
using BlazorServerAuthIssue.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BlazorServerAuthIssue.AuthServices
{
    public class CustomAuthenticationStateProvider : AuthenticationStateProvider
    {
        private readonly ILoginService _loginService;
        private readonly ILocalStorageService _localStorageService;
        private readonly IConfiguration _configuration;
        public CustomAuthenticationStateProvider(ILocalStorageService localStorageService,
                                                 IConfiguration configuration,
                                                 ILoginService loginService)
        {
            _localStorageService = localStorageService;
            _loginService = loginService;
            _configuration = configuration;
        }
        public async override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            Log.Information("===> Checking authentications state...");
            //Check if jwt exists and it is valid
            UserModel currentUser = await GetUserByJWTAsync();

            if (currentUser != null && currentUser.UserName != null)
            {
                // TODO - Add Role claims
                //create a claims
                var claimEmailAddress = new Claim(ClaimTypes.Name, currentUser.UserName);
                var claimNameIdentifier = new Claim(ClaimTypes.NameIdentifier, Convert.ToString(currentUser.UserId));
                //create claimsIdentity
                var claimsIdentity = new ClaimsIdentity(new[] { claimEmailAddress, claimNameIdentifier }, "serverAuth");
                //create claimsPrincipal
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                Log.Information($"User {currentUser.UserName} authenticated via token");
                return new AuthenticationState(claimsPrincipal);
            }
            else
            {
                Log.Warning("===> Authentication state cleared.");
                return new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
            }
        }
        public async Task<UserModel> GetUserByJWTAsync()
        {
            try
            {
                var returnedUser = new UserModel();
                //Reading the token from localStorage
                var jwtToken = await _localStorageService.GetItemAsStringAsync("jwt_token");

                if (jwtToken == null)
                {
                    Log.Information("===> No token was found in local storage");
                    return null;
                }
                Log.Information("===> Validating existing token.");
                //getting the secret key
                string secretKey = _configuration["JWTSettings:SecretKey"];
                var key = Encoding.ASCII.GetBytes(secretKey);
                //preparing the validation parameters
                var tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var tokenHandler = new JwtSecurityTokenHandler();
                SecurityToken securityToken;

                //validating the token
                var sToken = jwtToken.Trim('"');  /// Remove quotes
                var principle = tokenHandler.ValidateToken(sToken, tokenValidationParameters, out securityToken);
                var jwtSecurityToken = (JwtSecurityToken)securityToken;

                if (jwtSecurityToken != null && jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    // TODO - Read claims
                    var username = jwtSecurityToken.Claims.First(c => c.Type == "nameid").Value;
                    if (username != null)
                    {
                        return await _loginService.ValidateUserUserName(username);
                    }
                    return null;
                }

                return null;

            }
            catch (Exception ex) when (ex.Message.ToString().Contains("IDX10223"))
            {
                Log.Warning(ex.ToString());
                Log.Warning("Token Expired. Removing...");
                await _localStorageService.RemoveItemAsync("jwt_token");
                return null;
            }
            catch (Exception ex)
            {
                //logging the error and returning null
                Log.Fatal("Exception : " + ex.Message);
                return null;
            }
        }
    }
}
