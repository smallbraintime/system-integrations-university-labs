using lab8.Entities;
using lab8.Model;

namespace lab8.Services.Users
{
		public interface IUserService
		{
				AuthenticationResponse Authenticate(AuthenticationRequest request);
				IEnumerable<User> GetUsers();
				User GetByUsername(string username);
				User GetById(int id);
		}
}
