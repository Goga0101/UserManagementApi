using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Security.Claims;

namespace UserManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;

        public UsersController(IUserService userService, ILogger<UsersController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDto dto)
        {
            try
            {
                
                await _userService.RegisterUserAsync(dto);
                _logger.LogInformation($"მომხმარებლის რეგისტრაცია წარმატებით დასრულდა {dto.Username}", dto.Username);
                return Ok("მომხმარებელი რეგისტრირებულია");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"მომხმარებლის {dto.Username} რეგისტრაციისას მოხდა შეცდომა", dto.Username);
                return StatusCode(500, "მომხმარებელის რეგისტრაციისას მოხდა შეცდომა");
            }
        }

       

        [HttpPost("login")]
        [SwaggerOperation("login for user")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]
        public async Task<IActionResult> Login(UserLoginDto dto)
        {
            try
            {
                var result = await _userService.LoginUserAsync(dto);
                _logger.LogInformation($"მომხმარებელი დალოგინდა {dto.Username}", dto.Username);
                return Ok(result);
            }
            catch(Exception ex) 
            {
                _logger.LogError(ex, $"მომხმარებლის დალოგინებისას მოხდა შეცდომა {dto.Username}", dto.Username);
                return BadRequest(ex.Message); 
            }
          
        }

        [HttpPost("refresh")]
        [SwaggerOperation("generate access and refresh token")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success")]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Bad Request")]

        public async Task<IActionResult> Refresh(TokenResponseDto tokenDto)
        {
            try
            {
                var result = await _userService.RefreshTokenAsync(tokenDto.RefreshToken);
                return Ok(result);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
           
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var profile = await _userService.GetUserProfileAsync(userId);

            return Ok(profile);
        }

        [Authorize]
        [HttpPut("profile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UserProfileDto dto)
        {
           
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                await _userService.UpdateUserProfileAsync(userId, dto);
                _logger.LogInformation($"მომხმარებელმა განაახლა მონაცმეები {dto.Username}", dto.Username);
                return Ok();
               
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"მონაცემების განახლებისას მოხდა შეცდომა  {dto.Username}", dto.Username);
                return BadRequest(ex.Message);
            }
        }
    }
}
