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
    public async Task<IActionResult> GetUsers(UserFilterDto filterDto,int page=1, int pageSize=20)
    {
        var user_list = await _userService.GetAllUsersAsync(filterDto,page,pageSize);
        if (user_list == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("no user is present!"));
        }
        return Ok(ApiResponse<PagedResult<UserResponseDto>>.SuccessResponse(user_list,"List of all user present!"));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(int id)
    {
        var found_user = await _userService.GetUserByIdAsync(id);

        if (found_user == null)
        {
            return NotFound(ApiResponse<string>.ErrorResponse("no user of this id is present!"));
        }
        return Ok(ApiResponse<User>.SuccessResponse(found_user,"user found!!"));
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        var  createdUser = await _userService.CreateUserAsync(dto);
        return CreatedAtAction(
            nameof(GetUserById),
            new { id = createdUser.UserId},
            ApiResponse<User>.SuccessResponse(createdUser,"user is created!")
        );
    }

}