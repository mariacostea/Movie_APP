using Microsoft.AspNetCore.Mvc;
using MobyLabWebProgramming.Core.DataTransferObjects;
using MobyLabWebProgramming.Core.Handlers;
using MobyLabWebProgramming.Core.Responses;
using MobyLabWebProgramming.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using MobyLabWebProgramming.Infrastructure.Authorization;
using MobyLabWebProgramming.Core.Enums;
using System.Net; // Pentru HttpStatusCode
using MobyLabWebProgramming.Core.Errors; // Pentru ErrorMessage

namespace MobyLabWebProgramming.Backend.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class AuthorizationController(IUserService userService, ILoginService loginService) : BaseResponseController
{
    private readonly ILoginService _loginService = loginService;

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RequestResponse>> Register([FromBody] RegisterDTO register)
    {
        var hashedUser = new UserAddDTO
        {
            Name = register.Username,
            Email = register.Email,
            Password = PasswordUtils.HashPassword(register.Password),
            Role = UserRoleEnum.User
        };

        return FromServiceResponse(await userService.AddUser(hashedUser));
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<RequestResponse>> ConfirmEmail([FromQuery] string token)
    {
        return FromServiceResponse(await userService.ConfirmEmail(token));
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult<RequestResponse>> Login([FromBody] LoginDTO login)
    {
        var user = await userService.GetUserByEmail(login.Email);
        if (user == null)
            return ErrorMessageResult(new ErrorMessage(HttpStatusCode.NotFound, "Emailul introdus nu există."));

        if (!PasswordUtils.VerifyPassword(login.Password, user.Password))
            return ErrorMessageResult(new ErrorMessage(HttpStatusCode.Unauthorized, "Parola introdusă este greșită."));

        if (!user.EmailConfirmed)
            return ErrorMessageResult(new ErrorMessage(HttpStatusCode.Unauthorized, "Confirmă adresa de email."));

        var userDTO = new UserDTO
        {
            Id = user.Id,
            Email = user.Email,
            Name = user.Name,
            Role = user.Role
        };

        var token = _loginService.GetToken(userDTO, DateTime.UtcNow, TimeSpan.FromDays(7));

        return Ok(RequestResponse.Success(token));

    }
}
