using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using DevbitApi.Entities;
using DevbitApi.Helpers;
using DevbitApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;

namespace DevbitApi.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserContext _context;
        private readonly AppSettings _appSettings;

        //public List<UserModel> authUsers;

        public UserController(UserContext context, IOptions<AppSettings> appSettings)
        {
            _context = context;
            _appSettings = appSettings.Value;
        }

        [AllowAnonymous]
        [HttpPost("logout")]
        public List<UserModel> logout(string userName)
        {
            //return authUsers;
            return null;
        }

        [AllowAnonymous]
        [HttpPost("Login")]
        public async Task<UserModel> Login(string userName, string userPassword)
        {
            var users = await GetAllUsers();
            UserModel r = new UserModel() { UserName = "1", UserPassword = "2" , id = 0, Email = "3"};
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
                            new Claim(ClaimTypes.Name, user.id.ToString())
                        }),
                        Expires = DateTime.UtcNow.AddMinutes(10),
                        SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
                    };
                    var token = tokenHandler.CreateToken(tokenDescriptor);
                    user.Token = tokenHandler.WriteToken(token);
                    //authUsers.Add(user);
                    r = user;
                }
            }
            return r;
        }

        [AllowAnonymous] //for tests only
        [HttpDelete("delete")]
        public async Task<string> DeleteUser(string userName)
        {
            string response = "User Deleted";
            HttpClient client = new HttpClient();

            try
            {
                var users = await GetAllUsers();

                var y = users.FirstOrDefault(x => x.UserName == userName);

                if (y != null)
                {
                    await client.DeleteAsync($"https://develop.particula.devbitapp.be/users/{userName}");
                }
                else
                {
                    response = "User Not Found";
                }
            }
            catch (Exception e)
            {
                response = e.ToString();
            }

            return response;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<string> RegisterUser(string userName, string userPassword, string email)
        {
            HttpClient client = new HttpClient();
            string responseString;

            var users = await GetAllUsers();

            var find = users.FirstOrDefault(x => x.UserName == userName);

            if (find == null) 
            {
                var values = new Dictionary<string, string>
                {
                    { "UserName", userName },
                    { "UserPassword", userPassword },
                    { "Email", email}
                };

                var content = new FormUrlEncodedContent(values);

                var response = await client.PostAsync("https://develop.particula.devbitapp.be/users", content);

                //responseString = await response.Content.ReadAsStringAsync();
                responseString = "User Successfully Created";
            }
            else
            {
                responseString = "User already exists";
            }

            

            return responseString;
        }

        
        private async Task<List<UserModel>> GetAllUsers()
        {
            HttpClient clients = new HttpClient();
            HttpResponseMessage responses = await clients.GetAsync("https://develop.particula.devbitapp.be/users");
            responses.EnsureSuccessStatusCode();
            string responseBody = await responses.Content.ReadAsStringAsync();

            List<UserModel> result = JsonConvert.DeserializeObject<List<UserModel>>(responseBody);
            
            return result;
        }

        [AllowAnonymous] //only for tests
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


        //[AllowAnonymous]
        //[HttpPost("register")]
        //public async Task<ActionResult<UserModel>> PostUser([FromBody]UserModel userModel)
        //{
        //    _context.UserModels.Add(userModel);
        //    await _context.SaveChangesAsync();

        //    return userModel;
        //}


    }
}
