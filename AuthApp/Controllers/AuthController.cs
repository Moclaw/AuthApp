using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using System;
using AuthApp.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using AuthApp.Data;
using System.Linq;
using System.Collections.Generic;
using AuthApp.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using AuthApp.Models.Respones;
using AuthApp.Models.Request;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using AuthApp.Models.Interface;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/auth/[action]")]
    public class AuthController : BaseController
    {
        public AuthController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService , IAuthServices auth) : base(context, userManager, roleManager, configuration, mailService,auth)
        {
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Register([FromBody] RegisterViewModel userDetails)
        {
            try
            {
                if (!ModelState.IsValid || userDetails == null)
                {
                    return new BadRequestObjectResult(new { Message = "User Registration Failed" });
                }
                var checkMail = await _userManager.FindByEmailAsync(userDetails.Email);
                if (checkMail != null)
                {
                    return new BadRequestObjectResult(new { Message = "Email Already!" });
                }
                var identityUser = new IdentityUser() { UserName = userDetails.Username, Email = userDetails.Email };
                var result = await _userManager.CreateAsync(identityUser, userDetails.Password);
                if (!result.Succeeded)
                {
                    var dictionary = new ModelStateDictionary();
                    foreach (IdentityError error in result.Errors)
                    {
                        dictionary.AddModelError(error.Code, error.Description);
                    }

                    return new BadRequestObjectResult(new { Message = "User Registration Failed", Errors = dictionary });
                }
                var role = userDetails.Role == "Admin" ? "Admin" : "User";
                await _userManager.AddToRoleAsync(identityUser, role);
                return Ok(new { Message = "Registration success" });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
        [HttpPost]
        public async Task<IActionResult> Login([FromBody] LoginViewModel credentials)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (credentials != null)
                    {
                        var identityUser = await _userManager.FindByEmailAsync(credentials.Email);
                        var result = await _userManager.CheckPasswordAsync(identityUser, credentials.Password);
                        if (identityUser == null || result != true)
                        {
                            return BadRequest(new AuthResult()
                            {
                                Errors = new List<string>()
                                    {
                                        "Login Fail"
                                    },
                                Success = false
                            });
                        }
                        var userRole = _context.Roles.FirstOrDefault(c => c.Id == (_context.UserRoles.FirstOrDefault(a => a.UserId == identityUser.Id).RoleId));
                        if (userRole.Name != "Admin")
                        {
                            var sendMail =  _mailService.SendEmailAsync(identityUser.Email);
                            if (sendMail != true)
                            {
                                return BadRequest(new AuthResult()
                                {
                                    Errors = new List<string>()
                                    {
                                        "Login Fail"
                                    },
                                    Success = false
                                });
                            }
                        }
                        
                        var token = authServices.GenerateToken(identityUser);
                        return Ok(token);
                    }
                }
                return Ok();

            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
    }
}
