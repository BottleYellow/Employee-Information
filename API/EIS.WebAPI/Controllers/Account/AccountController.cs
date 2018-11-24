using System.Collections.Generic;
using EIS.Entities.User;
using EIS.Repositories.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;
using System.Text;
using System;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using EIS.WebAPI.Filters;

namespace EIS.WebAPI.Controllers
{
   
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        public AccountController(IRepositoryWrapper repository) : base(repository)
        {
        }

        [HttpPost]
        [Route("login")]
        public IActionResult Login(Users user)
        {
            string s = _repository.Users.ValidateUser(user);
            if (s == "success")
            {
                JwtSecurityToken token = GenerateToken(user.UserName);
                string s1 = new JwtSecurityTokenHandler().WriteToken(token);
                return Ok(token);
            }
                
            else
                return BadRequest("Username or password incorrect");
        }
        #region comment
        //[HttpPost("token")]
        //public IActionResult Token()
        //{
        //    var header = Request.Headers["Authorization"];
        //    if (header.ToString().StartsWith("Basic"))
        //    {
        //        var credValue = header.ToString().Substring("Basic ".Length).Trim();
        //        var usernameAndPassenc = Encoding.UTF8.GetString(Convert.FromBase64String(credValue));
        //        var usernameAndPass = usernameAndPassenc.Split(":");
        //        if (usernameAndPass[0] == "Admin" && usernameAndPass[1] == "pass")
        //        {
        //            var claimsdata = new[] { new Claim(ClaimTypes.Name, "username"), new Claim(ClaimTypes.Role, "Admin") };
        //            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("askjdkasdakjsdaksdasdjaksjdadfgdfgkjdda"));
        //            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
        //            var token = new JwtSecurityToken(
        //                issuer: "mysite.com",
        //                audience: "mysite.com",
        //                expires: DateTime.Now.AddMinutes(1),
        //                claims: claimsdata,
        //                signingCredentials: signInCred
        //                );
        //            var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
        //            return Ok(tokenstring);
        //        }
        //    }
        //    return BadRequest("wrong request");
        //}
        #endregion

        // GET: api/Logins
        
        [HttpGet]
        public IEnumerable<Users> GetLogins()
        {
            return _repository.Users.FindAll();
        }

        // GET: api/Logins/5
        [HttpGet("{id}")]
        public IActionResult GetLogin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = _repository.Users.FindByCondition(x => x.Id == id);

            if (login == null)
            {
                return NotFound();
            }

            return Ok(login);
        }

        // PUT: api/Logins/5
        [HttpPut("{id}")]
        public IActionResult PutLogin([FromRoute] int id, [FromBody] Users login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != login.Id)
            {
                return BadRequest();
            }
            _repository.Users.Update(login);

            try
            {
                _repository.Users.Save();
            }
            catch (DbUpdateConcurrencyException)
            {
                
                    throw;
                
            }

            return NoContent();
        }

        // POST: api/Logins
        [HttpPost]
        public IActionResult PostLogin([FromBody] Users login)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            _repository.Users.Create(login);
            _repository.Users.Save();
            return CreatedAtAction("GetLogin", new { id = login.Id }, login);
        }

        // DELETE: api/Logins/5
        [HttpDelete("{id}")]
        public IActionResult DeleteLogin([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var login = _repository.Users.FindByCondition(x => x.Id == id);
            if (login == null)
            {
                return NotFound();
            }
            _repository.Users.Delete(login);
            _repository.Users.Save();
            return Ok(login);
        }

        public JwtSecurityToken GenerateToken(string username)
        {
            var claimsdata = new[] { new Claim(ClaimTypes.Name, username) };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("askjdkasdakjsdaksdasdjaksjdadfgdfgkjdda"));
            var signInCred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);
            TimeZoneInfo INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var token = new JwtSecurityToken(
                issuer: "mysite.com",
                audience: "mysite.com",
                notBefore: TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE),
                expires: TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, INDIAN_ZONE).AddDays(1),
                claims: claimsdata,
                signingCredentials: signInCred
                );
            var tokenstring = new JwtSecurityTokenHandler().WriteToken(token);
            return token;   
        }
    }
}