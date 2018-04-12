using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Matricks.Data;
using Matricks.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Matricks.Controllers
{
    [Produces("application/json")]
    [Route("api/Auth")]
    public class AuthController : Controller
    {
        private readonly IAuthRepository _repo;

        private readonly IConfiguration _config;

        public AuthController(IAuthRepository repo, IConfiguration config)
        {
            _repo = repo;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDTO user) 
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // Make user name lower case
            user.UserName = user.UserName.ToLower();
            
            // If duplicate user name and return bad request here
            // Need method in AuthRepo to test for this
            //User Validation
            if (_repo.ValidateUserName(user.UserName))
            {
                ModelState.AddModelError("UserName", "User name already exists");
                return BadRequest(ModelState);
            }

            var newUser = await _repo.Register(user.UserName, user.Password);
            // Temporary return result for testing
            return StatusCode(201, new { ID = newUser.ID, UserName = newUser.UserName });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO user)
        {
            var storedUser = await _repo.Login(user.UserName, user.Password);
            if (storedUser == null)
            {
                return Unauthorized();
            }

            //Create token handler and encryption key
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("TokenSettings:JWTKey").Value);
            //Create the token descriptor
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[] {
                    new Claim(ClaimTypes.NameIdentifier, storedUser.ID.ToString()),
                    new Claim(ClaimTypes.Name, storedUser.UserName)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha512Signature)
            };
            //Generate the token and convert it to a string
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var tokenString = tokenHandler.WriteToken(token);


            // Temporary return value for testing
            return Ok(new { tokenString = tokenString, ID = storedUser.ID, UserName = storedUser.UserName });
        }
    }
}