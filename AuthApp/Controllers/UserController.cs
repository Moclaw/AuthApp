using AuthApp.Data;
using AuthApp.Models;
using AuthApp.Models.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/user/[action]")]
    [Authorize(Roles = "User, Admin")]
    public class UserController : BaseController
    {
        public UserController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService) : base(context, userManager, roleManager, configuration, mailService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetUserDetail()
        {
            try
            {
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                return Ok(user);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }

        }
        [HttpPost]
        public async Task<IActionResult> ChangeInformation([FromBody] ChangeUserDetailViewModel userDetails)
        {
            try
            {
                if (!ModelState.IsValid || userDetails == null)
                {
                    return new BadRequestObjectResult(new { Message = "User Change Failed" });
                }
                var user = await _userManager.FindByNameAsync(User.Identity.Name);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { Message = "User Change Failed" });
                }
                if (userDetails.Username != user.UserName)
                {
                    user.UserName = userDetails.Username;
                }
                user.Email = userDetails.Email;
                await _userManager.ChangePasswordAsync(user, userDetails.Password, userDetails.OldPassword);
                var result = await _userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    var dictionary = new ModelStateDictionary();
                    foreach (IdentityError error in result.Errors)
                    {
                        dictionary.AddModelError(error.Code, error.Description);
                    }

                    return new BadRequestObjectResult(new { Message = "User Change Failed", Errors = dictionary });
                }
                return Ok(new { Message = "User Change Success" });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}
