using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace AuthApp.Models.Interface
{
    public interface IAuthServices
    {
        object GenerateToken(IdentityUser identityUser);
    }
}
