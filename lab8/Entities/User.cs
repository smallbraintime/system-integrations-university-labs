using System.Text.Json.Serialization;

namespace lab8.Entities
{
		public class User
		{
				public int Id { get; set; }
				public string Username { get; set; }
				[JsonIgnore]
				public string Password { get; set; }
				public List<Role> Roles { get; set; }
		}
}
