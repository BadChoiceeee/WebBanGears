using Microsoft.AspNetCore.Identity;

namespace WebBanGear.Models
{
	public class AppUserModel : IdentityUser
	{
		public string Occupation {  get; set; } // nghề nghiệp
		public string RoleId { get; set; }
        public string Token { get; set; }

    }
}
	