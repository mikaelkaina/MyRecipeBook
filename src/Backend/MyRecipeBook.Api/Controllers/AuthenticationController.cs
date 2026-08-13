using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Login.WithEmailAndPassword;
using MyRecipeBook.Application.UseCases.PasswordRecovery.RequestCode;
using MyRecipeBook.Communication.Requets.User;
using MyRecipeBook.Communication.Requets.VerificationCode;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegisteredUserJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Login(
        [FromServices] ILoginWithEmailAndPasswordUseCase usecase,
        [FromBody] RequestLoginJson request)
    {
        var response = await usecase.Execute(request);

        return Ok(response);
    }

    [HttpPost("password-recovery")]
    [ProducesResponseType( StatusCodes.Status202Accepted)]
    public async Task<IActionResult> PasswordRecovery(
        [FromServices] IRequestPasswordRecoveryCodeUseCase usecase,
        [FromBody] RequestPasswordRecoveryJson request)
    {
        await usecase.Execute(request);

        return Accepted();
    }
}
