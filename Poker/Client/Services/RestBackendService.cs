using Poker.Shared.Models.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Poker.Client.Services
{

    public class RestBackendService
    {
        private readonly HttpClient _httpClient;

        public RestBackendService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<LoggedInUser> CheckLogin(LoginData loginData)
        {
            try
            {
                var result = await _httpClient.PostAsJsonAsync<LoginData>("api/login", loginData);
                return await result.Content.ReadFromJsonAsync<LoggedInUser>();
            }
            catch
            {
                return null;
            }

        }

        public async Task Logout(LoggedInUser loggedInUser)
        {
            await _httpClient.DeleteAsync($"api/login/{loggedInUser.Id}");
        }
    }
}
