using Microsoft.AspNetCore.Mvc;
using RecipeApp.Shared;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RecipeController : ControllerBase
    {
        [HttpGet]
        [Route("GetAllRecipes")]
        public List<RecipeDto> GetAllRecipes()
        {
            // get data from entity framework

            return MockData.MockRecipes;
            
        }

        [HttpGet]
        [Route("GetById")]
        public RecipeDto GetById(Guid recipeId)
        {
            // get data from entity framework
            return new RecipeDto();
        }
    }
}