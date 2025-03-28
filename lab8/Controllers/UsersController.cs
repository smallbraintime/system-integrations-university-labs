using lab8.Services.Users;
using Microsoft.AspNetCore.Mvc;
using lab8.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace lab8.Controllers
{
		[Route("api/[controller]")]
		[ApiController]
		public class UsersController : ControllerBase
		{
				private IUserService userService;

				public UsersController(IUserService userService)
				{
						this.userService = userService;
				}

				[HttpPost("authenticate")]
				public IActionResult Autheticate(AuthenticationRequest request)
				{
						var response = userService.Authenticate(request);

						if (response == null)
						{
								return BadRequest(new { message = "Username or password is incorrect" });
						}
						return Ok(response);
				}

				[HttpGet("getallusers")]
				[Authorize(Roles = "admin", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
				public IActionResult GetAllUsers()
				{
						var response = userService.GetUsers();
						return Ok(response);
				}

				[HttpGet("countusers")]
				[Authorize(Roles = "user", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
				public IActionResult CountUsers()
				{
						var response = userService.GetUsers();
						return Ok(response.Count());
				}
		}
}
