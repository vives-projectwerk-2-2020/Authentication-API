using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using DevbitApi.Entities;
using DevbitApi.Models;
using DevbitApi.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using RestSharp;

namespace DevbitApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserService _userService;
        private readonly UserContext _context;

        public UserController(IUserService userService, UserContext context)
        {
            _userService = userService;
            _context = context;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public IActionResult Autenticate([FromBody]AuthenticateModel model)
        {
            var user = _userService.Authenticate(model.Username, model.Password);

            if (user == null)
            {
                return BadRequest(new { message = "Username or password is incorrect" });
            }

            return Ok(user);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var client = new RestClient($"http://develop.particula.devbitapp.be:8080/users");
            var request = new RestRequest(Method.GET);
            IRestResponse response = await client.ExecuteAsync(request);

            string resp = response.ToString();
            var json = JsonConvert.SerializeObject(response.Content);
            //TODO: transform the response here to suit your needs

            return Ok(json);
        }

        /// <summary>
        /// Token is user-based
        /// use this function to check current userid with the token (got from login)
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public User Test()
        {
            var claimsIdentity = this.User.Identity as ClaimsIdentity;
            var userId = claimsIdentity.FindFirst(ClaimTypes.Name)?.Value;
            User test = _userService.GetCurrentUser(Convert.ToInt32(userId));
            return test;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserModel>> PostUser([FromBody]UserModel userModel)
        {
            _context.UserModels.Add(userModel);
            await _context.SaveChangesAsync();

            return userModel;
        }


    }
}
