using Microsoft.AspNetCore.Identity;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthApp.Models
{
    public class UserToken
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Token { get; set; }
        public DateTime AddedDate { get; set; }
        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }
    }
}
