using FraudDetection.API.Data;
using FraudDetection.API.DTOs;
using FraudDetection.API.Models;
using FraudDetection.API.Services;
using Microsoft.AspNetCore.Mvc;
// using ]
using  Microsoft.AspNetCore.Authorization;

namespace FraudDetection.API.Controllers;


// need to remove createdUser as it contains password Hash.. 
// create Response DTO.


[ApiController]
[Route("api/[Controller]")]
[Authorize]
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
        if (user_list == null)
        {
            return NotFound(null);
        }
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
    public async Task<IActionResult> CreateUser(CreateUserDtos dto)
    {
        var  createdUser = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.UserId},
            createdUser
        );
    }

}