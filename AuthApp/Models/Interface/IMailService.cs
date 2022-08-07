using AuthApp.Models;
using System.Threading.Tasks;

namespace AuthApp.Models.Interface
{
    public interface IMailService
    {
        bool SendEmailAsync(string toEmail);
    }
}
