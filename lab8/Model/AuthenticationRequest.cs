using System.ComponentModel.DataAnnotations;

namespace lab8.Model
{
		public class AuthenticationRequest
		{
				[Required]
				public required string Username { get; set; }
				[Required]
				public required string Password { get; set; }
		}
}
