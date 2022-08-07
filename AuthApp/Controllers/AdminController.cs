using AuthApp.Data;
using AuthApp.Models.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApp.Controllers
{
    [ApiController]
    [Route("api/admin/[action]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : BaseController
    {
        public AdminController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService) : base(context, userManager, roleManager, configuration, mailService)
        {
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUser()
        {
            try
            {
                var users = await _context.Users.ToListAsync();
                if (users == null)
                {
                    return new BadRequestObjectResult(new { Message = "No User Found" });
                }
                return Ok(users);
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }
        [HttpPost]
        public async Task<IActionResult> DeleteUser([FromForm] string userId)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    return new BadRequestObjectResult(new { Message = "User Not Found" });
                }
                await _userManager.DeleteAsync(user);
                _context.UserTokens.RemoveRange(_context.UserTokens.Where(u => u.UserId == userId));
                return Ok(new { Message = "User Deleted" });
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

    }
}
