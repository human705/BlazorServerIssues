using Blazored.LocalStorage;
using BlazorServerAuthIssue.AuthServices;
using BlazorServerAuthIssue.Models;
using Microsoft.AspNetCore.Components.Authorization;

namespace BlazorServerAuthIssue.Services
{
    public static class ServiceRegistration
    {
        public static void AddMyServices(this IServiceCollection services)
        {
            services.AddBlazoredLocalStorage();
            services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
            services.AddScoped<IUserRepository, MockUserRepository>();
            services.AddScoped<ILoginService, MockLoginService>();


        }
    }
 }
