using IdentityWithJWt.Data;
using IdentityWithJWT.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IdentityWithJWT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private UserManager<ApplicationUser> userManager;


        public AuthController(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            var user = await userManager.FindByNameAsync(model.UserName);
            if(user != null && await userManager.CheckPasswordAsync(user, model.Password)) {


                var userRoles = await userManager.GetRolesAsync(user);
                var claims = new List<Claim>
              {
                  new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
                  new Claim (JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()) 
              };

                foreach (var role in userRoles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                var signinKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("AspNetCoreDersim"));
                var token = new JwtSecurityToken(
                    issuer: "https://localhost:44388",
                    audience: "https://localhost:44388",
                    expires: DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Utc).AddHours(1), 
                    claims: claims,
                    signingCredentials: new SigningCredentials(signinKey, SecurityAlgorithms.HmacSha256)
                    );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    message = "Login Successful"
                });
               
            }
            else
            {
                return BadRequest(new
                {
                    message = "Wrong username or password"
                });
            }


        }
    }
}
