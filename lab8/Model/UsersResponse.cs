using lab8.Entities;

namespace lab8.Model
{
		public class UsersResponse
		{
				public IEnumerable<User> Users { get; set; }

				public UsersResponse(IEnumerable<User> users)
				{
						Users = users;
				}
		}
}
