using AuthApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthApp.Models.Interface
{
    public interface IUserController
    {
        Task<IActionResult> ChangeInformation([FromBody] ChangeUserDetailViewModel userDetails);
        Task<IActionResult> GetUserDetail();
    }
}