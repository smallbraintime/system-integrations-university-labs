using lab8.Services.Users;
using Microsoft.AspNetCore.Mvc;
using lab8.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace lab8.Controllers
{
		[Route("api/[controller]")]
		[ApiController]
		public class NumberController : ControllerBase
		{
				private IUserService userService;

				public NumberController(IUserService userService)
				{
						this.userService = userService;
				}

				[HttpGet("drawnumber")]
				[Authorize(Roles = "number", AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
				public IActionResult DrawNumber()
				{
						return Ok(new int[] { 3, 5, 7, 11, 13 }[(new Random()).Next(0, 5)]);
				}
		}
}

