using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DevbitApi.Entities;
using DevbitApi.Helpers;
using DevbitApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DevbitApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly AppSettings _appSettings;

        public UserController(UserContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<string> Login(string userName, string userPassword)
        {
            var users = await GetAllUsers();
            string r = "false";

            foreach (var user in users)
            {
                if (user.UserName == userName && user.UserPassword == userPassword)
                {
                    var tokenHandler = new JwtSecurityTokenHandler();
                    var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
                    var tokenDescriptor = new SecurityTokenDescriptor
                    {
                        Subject = new ClaimsIdentity(new Claim[]
                        {
                    new Claim(ClaimTypes.Name, user.UserId.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddSeconds(10),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    user.Token = tokenHandler.WriteToken(token);

                    r = user.Token.ToString();
                }

                
            }

            return r;
        }

        
        private async Task<List<UserModel>> GetAllUsers()
        {
            HttpClient clients = new HttpClient();
            HttpResponseMessage responses = await clients.GetAsync("http://develop.particula.devbitapp.be:8080/users");
            responses.EnsureSuccessStatusCode();
            string responseBody = await responses.Content.ReadAsStringAsync();

            List<UserModel> result = JsonConvert.DeserializeObject<List<UserModel>>(responseBody);
            
            return result;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await GetAllUsers();

            return Ok(result);
        }

        /// <summary>
        /// Token is user-based
        /// use this function to check current userid with the token (got from login)
        /// </summary>
        /// <returns></returns>


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
