using FraudDetection.API.Data;
using FraudDetection.API.Models;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;
// using ]
namespace FraudDetection.API.Controllers;

[ApiController]
[Route("api/[Controller]")]
public class UserController :ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        var user_list = await _userService.GetAllUsersAsync();
        return Ok(user_list);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var found_user = await _userService.GetUserByIdAsync(id);

        if (found_user == null)
        {
            return NotFound();
        }
        return Ok(found_user);
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(User user)
    {
        var  createdUser = await _userService.CreateUserAsync(user);
        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.UserId},
            createdUser
        );
    }

}