using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Recipe.DeleteById;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Application.UseCases.Recipe.Recent;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.UpdateById;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses;

namespace MyRecipeBook.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class RecipesController : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(ResponseRegiteredRecipeJson), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromServices] IRecipeRegisterUseCase useCase,
        [FromBody] RequestRecipeJson request)
    {
        var result = await useCase.Execute(request);
        return Created(string.Empty, result);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseRegiteredRecipeJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, [FromServices] IGetRecipeByIdUseCase useCase)
    {
        var recipe = await useCase.Execute(id);
        return Ok(recipe);
    }

    [HttpGet("Recent")]
    [ProducesResponseType(typeof(ResponseRecipesJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetRecent([FromServices] IGetRecentRecipesUseCase useCase)
    {
        var recipes = await useCase.Execute();
        return Ok(recipes);
    }

    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteById([FromRoute] Guid id, [FromServices] IDeleteRecipeByIdUseCase useCase)
    {
        await useCase.Execute(id);
        return NoContent();
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateById([FromRoute] Guid id, [FromBody] RequestRecipeJson request,
        [FromServices] IRecipeUpdateByIdUseCase useCase)
    {
        await useCase.Execute(id, request);
        return NoContent();
    }
}
