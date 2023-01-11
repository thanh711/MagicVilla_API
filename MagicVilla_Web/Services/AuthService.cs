using MagicVilla_Utility;
using MagicVilla_Web.Models;
using MagicVilla_Web.Models.Dto;
using MagicVilla_Web.Services.IServices;

namespace MagicVilla_Web.Services
{
    public class AuthService : BaseService, IAuthService
    {
        private readonly IHttpClientFactory _clientFactory;
        private string villaUrl;

        public AuthService(IHttpClientFactory clientFactory, IConfiguration configuration) : base(clientFactory)
        {
            this._clientFactory = clientFactory;
            villaUrl = configuration.GetValue<string>("ServiceUrls:VillaAPI");
        }

        public Task<T> LoginAsync<T>(LoginRequestDTO loginRequestDTO)
        {
            return SendAsync<T>(
               new APIRequest()
               {
                   ApiType = SD.ApiType.POST,
                   Data = loginRequestDTO,
                   Url = villaUrl + "/api/v1/UsersAuth/login"
               });
        }

        public Task<T> RegisterAsync<T>(RegisterationRequestDTO userDTO)
        {
            return SendAsync<T>(
                new APIRequest()
                {
                    ApiType = SD.ApiType.POST,
                    Data = userDTO,
                    Url = villaUrl + "/api/v1/UsersAuth/register"
                });
        }
    }
}
