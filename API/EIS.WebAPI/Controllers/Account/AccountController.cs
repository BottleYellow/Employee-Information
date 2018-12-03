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
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net;
using Microsoft.Extensions.Caching.Distributed;


namespace EIS.WebAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : BaseController
    {
        protected IDistributedCache distributedCache;
        public AccountController(IRepositoryWrapper repository,IDistributedCache distributedCache) : base(repository)
        {
            this.distributedCache = distributedCache;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("login")]
        public IActionResult Login(Users user)
        {
            string s = _repository.Users.ValidateUser(user);
            if (s == "success")
            {
                Users u = _repository.Users.FindByUserName(user.UserName);
                JwtSecurityToken token = _repository.Users.GenerateToken(u.Id);
                string s1 = new JwtSecurityTokenHandler().WriteToken(token);
                int pid = u.PersonId;
                if (s1!= null)
                {

                    var options = new DistributedCacheEntryOptions();
                    options.SetAbsoluteExpiration(TimeSpan.FromMinutes(1));
                    distributedCache.SetString("TokenValue", s1, options);
                    distributedCache.SetString("PersonId", pid.ToString(), options);
                }
                AccessToken accessToken = new AccessToken()
                {
                    TokenName = s1,
                    UserId=u.PersonId
                };
                return Ok(accessToken);
            }

            else
            {
                AccessToken accessToken = new AccessToken()
                {
                    TokenName = null
                    
                };
                return NotFound(accessToken);
            }
                 
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

        [HttpPost]
        [Route("logout/{id}")]
        public IActionResult Logout(int id)
        {
            //int n=_repository.Users.RemoveToken(id);
            //if (n > 0)
            //{
            //    return Ok("Successfully Logged out.");
            //}
            //else
            //{
            //    return BadRequest();
            //}
            return BadRequest();
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
    }
}