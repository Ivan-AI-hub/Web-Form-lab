using Microsoft.AspNetCore.Identity;

namespace Whosales.Web.Models
{
    public class User : IdentityUser
    {
        public int Year { get; set; }
    }
}
