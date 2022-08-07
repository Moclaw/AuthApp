using AuthApp.Data;
using AuthApp.Models.Interface;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AuthApp.Controllers
{
    public class BaseController : Controller
    {
        protected readonly ApplicationDbContext _context;
        protected readonly UserManager<IdentityUser> _userManager;
        protected readonly RoleManager<IdentityRole> _roleManager;
        protected IConfiguration Configuration { get; }
        protected readonly IMailService _mailService;
        protected readonly IAuthServices authServices;
        public BaseController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService, IAuthServices auth)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            Configuration = configuration;
            _mailService = mailService;
            authServices = auth;
        }

        public BaseController(ApplicationDbContext context, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IMailService mailService)
        {
            Configuration = configuration;
        }
    }

}
