using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyRecipeBook.Application.UseCases.Recipe.ChangeIllustration;
using MyRecipeBook.Application.UseCases.Recipe.DeleteById;
using MyRecipeBook.Application.UseCases.Recipe.Filter;
using MyRecipeBook.Application.UseCases.Recipe.GetById;
using MyRecipeBook.Application.UseCases.Recipe.Recent;
using MyRecipeBook.Application.UseCases.Recipe.Register;
using MyRecipeBook.Application.UseCases.Recipe.UpdateById;
using MyRecipeBook.Communication.Requets.Recipe;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Communication.Responses.Recipe;

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
        [FromForm] RequestRecipeJson request,
        IFormFile? recipeIllustration)
    {
        var result = await useCase.Execute(request, recipeIllustration?.OpenReadStream());
        return Created(string.Empty, result);
    }

    [HttpPost("filter")]
    [ProducesResponseType(typeof(ResponseRecipesJson), StatusCodes.Status200OK)]
    public async Task<IActionResult> Filter([FromBody] RequestFilterRecipesJson? request,
        [FromServices] IFilterRecipesUseCase useCase)
    {
        var response = await useCase.Excute(request);
        return Ok(response);
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ResponseRegiteredRecipeJson), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById([FromRoute] Guid id, [FromServices] IGetRecipeByIdUseCase useCase)
    {
        var recipe = await useCase.Execute(id);
        return Ok(recipe);
    }

    [HttpPut("{id}/illustration")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResponseErrorJson), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ChangeIllustration(
        [FromRoute] Guid id, [FromForm] IFormFile recipeIllustration, 
        [FromServices] IChangeIllustrationUseCase useCase)
    {
        await useCase.Execute(id, recipeIllustration.OpenReadStream());

        return NoContent();
    }

    [HttpGet("recent")]
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
