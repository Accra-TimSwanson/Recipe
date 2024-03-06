using RecipeApp.Contracts;

namespace Recipe.Services
{
    public interface IRecipeService
    {
        Task<List<RecipeDto>> GetAllRecipes();
        Task<RecipeDto> GetRecipeById(Guid Id);
        Task<RecipeDto> AddRecipe(RecipeDto recipe);
        Task<RecipeDto> UpdateRecipe(RecipeDto recipe);
    }
}
