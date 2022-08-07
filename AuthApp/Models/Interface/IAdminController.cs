using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthApp.Models.Interface
{
    public interface IAdminController
    {
        Task<IActionResult> DeleteUser([FromForm] string userId);
        Task<IActionResult> GetAllUser();
    }
}