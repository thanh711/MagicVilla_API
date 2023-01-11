using MagicVilla_VillaAPI.Models;
using MagicVilla_VillaAPI.Models.Dto;
using MagicVilla_VillaAPI.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace MagicVilla_VillaAPI.Controllers
{
    [Route("api/v{version:apiVersion}/UsersAuth")]
    [ApiController]
    [ApiVersionNeutral]
    public class UsersController : Controller
    {
        private readonly IUserRepository userRepository;
        protected APIResponse _response;
        public UsersController(IUserRepository userRepository)
        {
            this.userRepository = userRepository;
            _response = new();
        }
        [HttpPost("login")]
        public async Task<ActionResult<APIResponse>> Login([FromBody] LoginRequestDTO model)
        {
            LoginResponseDTO loginResponse = await userRepository.Login(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username or password is incorrect");

                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            _response.Result = loginResponse;
            return Ok(_response);
        }
        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterationRequestDTO model)
        {
            bool isUniqueName = userRepository.IsUniqueUser(model.UserName);
            if (!isUniqueName)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Username allredy exist");

                return BadRequest(_response);
            }

            var user = userRepository.Register(model).Result;
            if (user == null)
            {
                _response.StatusCode = HttpStatusCode.BadRequest;
                _response.IsSuccess = false;
                _response.ErrorMessages.Add("Error while register");

                return BadRequest(_response);
            }
            _response.StatusCode = HttpStatusCode.OK;
            _response.IsSuccess = true;
            return Ok(_response);
        }
    }
}
