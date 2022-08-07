using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthApp.Models.Interface
{
    public interface IAuthController
    {
        Task<IActionResult> Login([FromBody] LoginViewModel credentials);
        Task<IActionResult> Register([FromBody] RegisterViewModel userDetails);
    }
}